USE StoreBSK;

CREATE TABLE Customer(
	id INTEGER PRIMARY KEY,
	name VARCHAR(20),
	surname VARCHAR(30)
);

CREATE TABLE Producer(
	id INTEGER PRIMARY KEY,
	name VARCHAR(20),
	address VARCHAR(50)
);

CREATE TABLE Article(
	name VARCHAR(20) PRIMARY KEY,
	description VARCHAR(50),
	producer_id INTEGER NOT NULL FOREIGN KEY REFERENCES Producer
);

CREATE TABLE Sale(
	id INTEGER PRIMARY KEY,
	netPrice DECIMAL(5,2),
	grossPrice DECIMAL(5,2),
	tax INTEGER,
	customer_id INTEGER NOT NULL FOREIGN KEY REFERENCES Customer
);

CREATE TABLE SpecificArticle(
	id INTEGER PRIMARY KEY,
	sale_id INTEGER NOT NULL FOREIGN KEY REFERENCES Sale,
	price DECIMAL(5,2),
	article_id VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Article
);

CREATE TABLE Users(
	id INTEGER PRIMARY KEY,
	login VARCHAR(30),
	password VARCHAR(64),
	salt VARCHAR(64),
	levelOfPermissions INTEGER NOT NULL
);

CREATE TABLE TablePermissions(
	nameOfTable VARCHAR(20),
	levelOfPermissions INTEGER,
	PRIMARY KEY(nameOfTable, levelOfPermissions)
);


INSERT INTO Customer VALUES (1, 'Jan', 'Nowak');
INSERT INTO Customer VALUES (2, 'Anna', 'Jantar');
INSERT INTO Customer VALUES (3, 'Robert', 'Lewandowski');

INSERT INTO Producer VALUES (1, 'Lidl', 'Skrzypeczna 77, 81-111 Gdynia');
INSERT INTO Producer VALUES (2, 'Biedronka', 'Pasieczna 717, 81-141 Gdynia');
INSERT INTO Producer VALUES (3, 'Tesco', 'Konopnicka 772, 81-131 Sopot');

INSERT INTO Article VALUES ('Chleb', 'Bardzo smaczny chleb', 1);
INSERT INTO Article VALUES ('Mleko', 'Nawet smaczne mleko', 1);
INSERT INTO Article VALUES ('Woda', 'Woda jak woda', 2);
INSERT INTO Article VALUES ('Pomidory', 'Nie no, pomidory to prima sort', 3);

INSERT INTO Sale VALUES (1, 123.23, 151.57, 23, 1);
INSERT INTO Sale VALUES (2, 33.25, 35.91, 8, 2);
INSERT INTO Sale VALUES (3, 245.99, 290.26, 18, 2);
INSERT INTO Sale VALUES (4, 321.56, 395.51, 23, 3);

INSERT INTO SpecificArticle VALUES (1, 1, 100.00,'Chleb');
INSERT INTO SpecificArticle VALUES (2, 1, 23.23,'Mleko');
INSERT INTO SpecificArticle VALUES (3, 2, 33.25, 'Woda');
INSERT INTO SpecificArticle VALUES (4, 3, 245.99, 'Pomidory');
INSERT INTO SpecificArticle VALUES (5, 4, 321.56, 'Pomidory');

--haslo(wszyscy to samo): alamakota
--SHA256
INSERT INTO Users VALUES (1, 'jan', 
'9D441570958B0B26BE3F77E92E0F71E89BD69387AC1AFEA1FD424E046B98FB8B',
'Lq!7KG28$&2vMSqe', 1);

INSERT INTO Users VALUES (2, 'ana', 
'D478987DEF2E8522369B981E8B6F7E03D7C61E93AC0DF64D07E5731247270BE7',
'GaXNfCQw@kg83plf', 2);

INSERT INTO Users VALUES (3, 'ben', 
'B7E4CC443169BE36F5DCF60D233C139921F92A58D68998FB56C2ECD983157603',
'LDO4RRheHb^DFpB3', 3);

INSERT INTO Users VALUES (4, 'john', 
'4376118E313AEFB0DE656BC9949798429FDFA494EF7F55CE03962B38A06F9B1B',
'K)9lk*L8)W3tdkIA', 4);

INSERT INTO TablePermissions VALUES ('Article', 1);
INSERT INTO TablePermissions VALUES ('SpecificArticle', 1);
INSERT INTO TablePermissions VALUES ('Sale', 3);
INSERT INTO TablePermissions VALUES ('Producer', 2);
INSERT INTO TablePermissions VALUES ('Customer', 4);
