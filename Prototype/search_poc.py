import json, re, itertools, time
import jaro
# import Levenshtein
from collections import defaultdict

class Verse:
    """Holds chapter/verse reference for a single verse"""
    def __init__(self, chapter, verse, text):
        self.chapter = chapter
        self.verse = verse
        self.text = text

    def get_word_at_index(self, index_start, word_count):
        return self.text[index_start : index_start + word_count]

    def __repr__(self):
        return f"Verse('{self.chapter}:{self.verse}')"


class VerseNGramMatch:
    def __init__(self, verse, word_index):
        self.verse = verse
        # The index at which the n-gram matched
        self.word_index = word_index
    
    def get_n_words_from_index(self, word_count):
        return " ".join(self.verse.get_word_at_index(self.word_index, word_count))

    def __repr__(self):
        return repr(self.verse)


class CorpusSearch():
    def __init__(self, chapters, tokenization_granularity=3, comparator=jaro.jaro_winkler_metric):
        self.search_index = {}
        self.string_comparator = comparator

        self.tokenization_granularity = tokenization_granularity
        for chapter in chapters:
            corpus = f"corpus2/_{chapter}.json"
            verses = self._clean_verses_from_corpus(corpus, chapter)

            index_for_chapter = self._build_search_index(verses)

            # Merge the dictionaries
            for key, val in index_for_chapter.items():
                if key not in self.search_index:
                    self.search_index[key] = []

                self.search_index[key] += val

    def perform(self, search_term, threshold=0):
        # Do search with the search term, assuming it is already parsed
        search_words = self._normalize_arabic_text(search_term).split(" ")
        search_term_tokens = map(lambda w: self.generate_n_grams(w, self.tokenization_granularity), search_words)

        ret = {}

        # Flatten the tokens nested array and iterate through them
        for token in itertools.chain(*search_term_tokens):
            # Look up in search index and score candidates
            candidate_verses = self.search_index.get(token, [])

            for candidate in candidate_verses:
                # Get lexical similarity for comparing input str to target
                lexical_similarity = max(self.string_comparator(search_term, candidate.get_n_words_from_index(i)) for i in range(len(search_words), len(search_words) + 1))

                if ret.get(candidate.verse, 0) < lexical_similarity and lexical_similarity > threshold:
                    ret[candidate.verse] = lexical_similarity

        return dict(sorted(ret.items(), reverse=True, key=lambda i: i[1]))

    def _build_search_index(self, tokenized_verses):
        inverted_index = defaultdict(lambda: [])

        for verse in tokenized_verses:
            chapter, verse_number, verse_words = verse["chapter"], verse["number"], verse["verse"]
            verse = Verse(chapter, verse_number, verse_words)

            for word_index, word in enumerate(verse_words):
                # Tokenize the verse's words and add to index
                tokens = self.generate_n_grams(word, self.tokenization_granularity)
                for token in tokens:
                    inverted_index[token].append(VerseNGramMatch(verse, word_index))

        return inverted_index

    def _clean_verses_from_corpus(self, corpus, chapter):
        ret = []
        with open(corpus) as f:
            verses = json.load(f)
            for verse in verses:
                words = self._normalize_arabic_text(verse["phonetic"], False).split(" ")
                verse_number = words[0]
                # Convert each word to its fixed format
                verse_words = list(filter(lambda x: len(x) > 0, map(self._fix_word, words[1:])))
                ret.append({
                    "chapter": chapter,
                    "number": int(verse_number.split(".")[1]),
                    "verse": verse_words
                })

        return ret

    def _normalize_arabic_text(self, text, discard_short_words=True):
        """Normalize the text by replacing diacritics with plain ASCII chars"""
        replacements = {
            'ī': 'i',
            '/': '',
            'ş': 's',
            'ž': 'z',
            'ĥ': 'h',
            'ū': 'u',
            'ţ': 't',
            'ā': 'a',
            'á': 'a',
            'đ': 'd',
            '-': ' ',
            '`': '',
            '\'': ''
        }
        text = text.lower()

        for letter in replacements:
            text = text.replace(letter, replacements[letter])

        # return text if discard_short_words == False else re.sub(r'\W*\b\w{1,2}\b', "", text)
        # return re.sub(r'\W*\b\w{1,2}\b', "", text)
        return re.sub(r'\b[a-zA-Z]{1,2}\b', "", text)

    @staticmethod
    def _fix_word(word):
        letters = re.findall("\D", word)

        # Remove non-letter marks
        for bad_letter in ('-', "'"):
            if bad_letter in letters:
                letters.remove(bad_letter)

        return "".join(letters)

    @staticmethod
    def generate_n_grams(word, n):
        ret = []
        for i in range(len(word)):
            ret.append(word[i:i + n])

        return ret

if __name__ == "__main__":
    comparators = {
        # "jaro L": Levenshtein.jaro_winkler,
        "jaro J": jaro.jaro_winkler_metric,
    }
    results = {}

    for comp_name, comparator in comparators.items():
        searcher = CorpusSearch(chapters=range(1, 115), tokenization_granularity=3, comparator=comparator)

        start = time.time()
        results[comp_name] = searcher.perform("rabbi zidni ilman", 0.85)
        print(time.time() - start, 'seconds')

    for comp_name, result in results.items():
        print(comp_name)

        for result, score in result.items():
            print("\t", result, score, " ".join(result.text))

    # import cProfile
    # print("Lev comparator")
    # searcher = CorpusSearch(chapters=range(1, 115), tokenization_granularity=3, comparator=lev)
    # cProfile.run('searcher.perform("alam najalil ardha mihada", 0.88)')

    # print("\n\n\n\n\nJaro comparator")
    # searcher = CorpusSearch(chapters=range(1, 115), tokenization_granularity=3, comparator=jaro.jaro_metric)
    # cProfile.run('searcher.perform("alam najalil ardha mihada", 0.88)')

    # print("\n\n\n\n\nHamming comparator")
    # searcher = CorpusSearch(chapters=range(1, 115), tokenization_granularity=3, comparator=hamming_distance)
    # cProfile.run('searcher.perform("alam najalil ardha mihada", 0.88)')
