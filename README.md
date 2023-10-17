# OIBIS
Projektni zadatak 13.
Implementirati komponentu za upravljanje sertifikatima (CertificateManagerService) koja obezbeduje:
• Kreiranje sertifikata (sa i bez privatnog kljuca). Napomena: Izgenerisane sertifikate treba duvati i u lokalnom folderu za potrebe distribucije.
• Povlacenje sertifikata u slucaju kompromitovanja, kao i izdavanje novo sertifikata.
Infrastruktura treba da sadrzi listu svih nevalidnih sertifikata (RevocationList). Takode, treba da obavesti sve klijente da je doslo do povladenja sertifikata, a klijent diji sertifikat je istekao ce instalirati novi sertifikat.
• Repliciranje podataka na backup server. Primarna i backup komponenta za upravlianje sertifikatima se autentifikuju koristeci Windows autentifikacioni protokol.
• Sve prethodno navedene akcije treba da se loguju u okviru CMS Windows event loga.
Razviti WCF klijent-servis model takav da se u&esnici u komunikaciii medusobno autentifikuju koristedi sertifikate generisane koris&enjem prethodno implementirane infrastrukture za upravljanje sertifikatima. Obostrana autentifikacija ove dve komponente se vrsi ChainTrust validacijom. Obe komponente se obracaju CertificateManagerService za dobijanje sertifikata, kao i za nihovo povladenje i obnavljanje u slucaju kompomitovanja. Distribucija setifikata se vr$i ru¿no.
Prilikom zahteva za dobijanje sertifikata, klijenti uspostavljaju komunikaciu sa CMS komponentom putem Windows autentifikacionog protokola. Na osnovu imena korisnika definise se CommonName atribut, dok se na osnovu pripadnosti Windows grupi definise OU atribut sertifikata
Nakon uspesne autentifikacije, klijenti se na random odredeni period [1-10 sec] javljaju servisu koji upisuje u tekstualni fajl sledece: "<ID>: <Timestamp>; <CommonName>" (gde je ID redni broj upisa u fajl, Timestamp je vreme poziva metode od strane klijenta, Common Name je atribut sertifikata) ali samo pod uslovom da je klijent sertifikovan da bude Elan iedne od detiri korisnicke grupe (RegionEast, Region West, RegionSouth, RegionNorth)
Dodatno, servis treba da vodi evidenciju o svim klijentskim procesima u okviru Application Windows event loga: 1) da je uspostavliena nova konekcia sa odredenim klijentom 2) da je komunikacija prekinuta.
