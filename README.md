# Evidencija prognozirane i ostvarene potrošnje električne energije
* Predmetni projekat iz predmeta Virtuelizacija procesa
## Opis projekta
Projekat predstavlja aplikaciju koja treba da se bavi evidencijom prognozirane i ostvarene potrošnje električne energije dobijene od klijenta kog predstavlja kompanija za prenos električne energije.

## Uvoz podataka o prognoziranoj i ostvarenoj potrošnji
Aplikacijom se uvoze podaci o planiranoj i ostvarenoj potrošnji električne energije. Uvoz se vrši iz CSV datoteka. Datoteke se importuju upisom željene putanje u konzolnu aplikaciju sa koje lokacije se čitaju sve CSV datoteke. Naziv datoteke se sastoji od tipa datoteke i datuma. Tip datoteke može biti "forecast" za prognoziranu potrošnju i "measured" za ostvarenu potrošnju. Podaci u svakom redu su sat na koji se potrošnja odnosi i iznos potrošnje u mW/h. Ako se datoteka odbacuje kao nevalidna, kreira se novi Audit objekat koji se upisuje u bazu podataka. Za svaki sat iz CSV datoteka kreira se po jedan objekat klase Load, ako objekat sa tim datumom i satom već ne postoji. Ako objekat sa pristiglim datumom i satom postoji, ažuriraju se Polja objekta na osnovu pristiglih podataka. Za svaku obrađenu CSV datoteku kreira se objekat ImportedFile i upisuje se u bazu podataka. 

## Proračun odstupanja između prognozirane i ostvarene potrošnje
Nakon što su učitani podaci upisani u bazu podataka, servis pristupa proračunu odstupanja između prognozirane i ostvarene potrošnje po satu. Izračunavanje se vrši samo za one objekte za koje su pristigli podaci i o prognoziranoj i ostvarenoj potrošnji. Odstupanje može da se izračunava kao apsolutno procentualno odstupanje ili kvadratno odstupanje. Odluka o tome da li će se koristiti apsolutno procentualno odstupanje ili kvadratno odstupanje donosi se na osnovu podešavanja u App.config datoteci servisnog dela aplikacije. Kada se završi proračun odstupanja i za posledji red, aktivira se događaj ažuriranja baze podataka proračunatim podacima. 

## Model podataka 
* Load (int Id, DateTime TimeStamp, double ForecastValue, double MeasuredValue, double AbsolutePercentageDeviation, double SquaredDeviation, int ImportedFileId)
* ImportedFile (int Id, string FileName)
* Audit (int Id, DateTime TimeStamp, MessageType MessageType, string Message)

## Implementacija baze podataka 
Baza podataka implementirana je kao XML baza podataka i kao InMemory baza podataka. 
* XML baza podataka sadrži XML datoteke u koje se upisuju podaci. Svaka tabela je implementirana kroz jednu XML datoteku. Ukoliko XML datoteka ne postoji, potrebno je da bude kreirana automatski.
* In-Memory baza podataka implementirana je kroz Dictionary struktura podataka. Svaka tabela je implementirana kroz jedan Dictionary, pri čemu je Key ID reda u tabeli, a Value je objekat odgovarajuće klase (Load, ImportedFile i Audit). Podaci u In-Memory bazi podataka postoje samo dok je servis pokrenut.

## Komponente aplikacije 
* Server  -  servisni sloj
* Client  -	korisnički interfejs, konzolna aplikacija
* Database  -  XML baza podataka i In-Memory baza podataka
* Common  -  projekat koji je zajednički za sve slojeve

## Autor
Dušan Borovićanin PR56/2020


