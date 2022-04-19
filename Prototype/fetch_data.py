from urllib.request import urlopen, Request
from bs4 import BeautifulSoup
import json, re


def download_chapter(chapter_number):
    print("Downloading chapter", chapter_number)
    url = f"http://sahih-bukhari.com/Pages/Quran/Quran_transliteration.php?id={chapter_number}"
    data = urlopen(url).read().decode("windows-1252")
    soup = BeautifulSoup(data, 'html.parser')
    chapter_text_table = soup.find_all('table')[-1]

    phonetic_text_array = []

    for table_row in chapter_text_table.find_all('tr'):
        rows = table_row.find_all('td')
        verse_number, verse_text = rows[0], rows[1]
        phonetic_text_array.append(verse_number.text + " " + verse_text.text)

    return phonetic_text_array


def save_chapter(chapter_number, data):
    with open(f'corpus2/_{chapter_number}.json', 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, indent=4)


def get_translations():
    translations_url = Request('https://quranenc.com/api/v1/translations/list', headers={'User-Agent': 'A'})
    translations = urlopen(translations_url).read()
    return filter(lambda x: 'english' in x['key'], json.loads(translations)['translations'])


def download_corpus_data():
    CORPUS_VERSE_DATA, _id = {}, 0

    for chapter_number in range(1, 115):
        CORPUS_VERSE_DATA[chapter_number] = []

        phonetic_text_array = download_chapter(chapter_number)

        for phonetic_verse in phonetic_text_array:
            CORPUS_VERSE_DATA[chapter_number].append({
                "id": _id,
                "chapter": chapter_number,
                "verse": None,
                "arabic_text": None,
                "phonetic": phonetic_verse,
                "footnotes": {},
                "translations": {},
            })

            _id += 1

        # Get all translations
        for translation in get_translations():
            language = translation['key']

            translation_url = Request(f'https://quranenc.com/api/v1/translation/sura/{language}/{chapter_number}', headers={'User-Agent': 'A'})
            translation_results = json.loads(urlopen(translation_url).read().decode('UTF-8'))['result']

            for idx, verse in enumerate(translation_results):
                CORPUS_VERSE_DATA[chapter_number][idx]['verse'] = int(verse["aya"])
                CORPUS_VERSE_DATA[chapter_number][idx]['arabic_text'] = verse["arabic_text"]

                cleaned_translation = re.sub(r'^.+\d+[).]?[  ]', '', verse['translation'].replace('\xa0', ' '))
                CORPUS_VERSE_DATA[chapter_number][idx]['translations'][language] = cleaned_translation
                try:
                    CORPUS_VERSE_DATA[chapter_number][idx]['footnotes'][language] = verse["footnotes"]
                except:
                    pass

        save_chapter(chapter_number, CORPUS_VERSE_DATA[chapter_number])


if __name__ == "__main__":
    download_corpus_data()
