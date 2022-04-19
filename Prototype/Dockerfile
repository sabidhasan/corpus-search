FROM python:3

COPY . /opt/app
WORKDIR /opt/app

RUN pip install -r requirements.txt

CMD [ "python", "/opt/app/search_poc.py" ]
