# Poprawki:

## Program nie działa  
Refaktor gdy kod nie działa jest problematyczny. 
Potrzebny jest nam punkt odniesienia by sprawdzić czy po naszym refaktorze kod dalej działa.
Najlepiej by było napisać testy, ale tym razem zrobimy to na oko, jako że interfejs klasy może się zmienić.
Poprawiamy:

Zła nazwa pliku / brak exception handling:  

    reader.ImportAndPrintData("dataa.csv"); - 

Zły warunek:

    for (int i = 0; i <= importedLines.Count; i++)

Brak sprawdzenia wielkości values:

    var values = importedLine.Split(';'); - 

Niepotrzebna wartość w inicjalizacji Listy:

    ImportedObjects = new List<ImportedObject>() { new ImportedObject() };

## Problemy z odpowiedzialnością klas/metod

Metoda

    public void ImportAndPrintData(string fileToImport, bool printData = true)

zawiera w sobie słowo "and" które sugeruje, że metoda ma dwie odpowiedzialności. 
Do tego dochodzi parametr bool, sugerujący kolejną odpowiedzialność, jednak ten nie jest w ogóle używany.

Dzielimy metodę na importującą i printującą, usuwamy zbędny parametr.

Klasa DataReader sama w sobie ciągle ma za duzo odpowiedzialności:
 - Odpowiada za konwersję danych tekstowych do kolekcji obiektów
 - Reprezentuje tą kolekcję obiektów
 - Printuje tą kolekcję

 Z antysymetrii klas i danych wiemy że:  
Dodanie nowych funkcjonalności do struktury jest proste, kosztem trudności dodawania nowych struktur.
Dodawanie nowych klas jest proste, kosztem trudności rozszerzania ich funkcjonalności.
W naszym przypadku możemy założyć większą szansę dodania nowych funkcjonalności niż zmiany struktury.

Tak więc:
- Użyjmy klasy DataReader do zwrócenia kolekcji obiektów z pliku.
- Niech sama kolekcja będzie osobnym tworem.
- Sam Datareader nie musi wiedzieć jak printować dane by pobrać je z pliku.
- Niech DataPrinter będzie osobną klasą - chcąc zmienić sposób wyświetlania zmienimy tylko tą klasę.  
  
Dzięki temu będziemy mogli rozszerzać możliwości przetwarzania tych danych bez zmiany klasy DataReader.
Teraz sprzężenie tych funkcjonalności jest mniejsze, ale wciąż może być dość duże.  

Możemy nowe klasy przenieść do nowego folderu i nowego namespace.

## Problem ze strukturami - data classes

ImportedObject to w zasadzie strukutra - zawiera tylko dane bez funkcjonalności. 
Niektórzy uważają struktury w OOP jako code smell. Ja nie mam zdania jeszcze.
To co wiem to to, że ImportedObject może sam na sobie wykonać niektóre operacje,
więc możemy przenieść te funkcjonalności do tej klasy.  

ImportedObject powtarza property "Name" z klasy bazowej, możemy to wyrzucić.
Do tego, Klasa bazowa ImportedObjectBaseClass raczej nie ma sensu, póki co scalam ją z drugą.  

Property i zmienne publiczne w klasie ImportedObject są zadeklarowane niekonsekwentnie, ujednolicamy do samych property.


## Problem z grupowaniem odpowiedzialności komentarzami

        // clear and correct imported data
        foreach (var importedObject in ImportedObjects)
        { ...

Komentarze tego typu sugerują zły podział odpowiedzialności.
Każdy taki komentarz możemy zamienić na prywatną metodę o podobnej nazwie co komentarz.
Kod powinien być czytelny nawet bez komentarzy.


## Długość linii i ilość zagnieżdżeń

Metoda printująca jest długa w poziomie. 
Dzielimy każdy foreach na osobną metodę.
 Możnaby też pokusić się o uogólnienie tych metod niezależnie od poziomu na którym jesteśmy ale to może wprowadzić bałagan,
 gdyż te funkcje po podzieleniu bardziej są poróżnione niż podobne, lepiej zostawić osobną funkcję dla każdej iteracji póki co.
 Możemy za to wydzielić powtózenia z tych funkcji.

 Niektóre linie są trochę za długie, rozdzielamy takie instrukcje na więcej linii.


## Problem ze switchem

Ciekawy case jest tutaj, niejawny switch:

            importedObject.Type = values[0];
            importedObject.Name = values[1];
            importedObject.Schema = values[2];
            importedObject.ParentName = values[3];
            importedObject.ParentType = values[4];
            importedObject.DataType = values[5];
            importedObject.IsNullable = values[6];

Drażni w oczy fakt, że wymieniamy kolejne wartości zamiast iterować po nich ale ponieważ przypisujemy je do struktury nie możemy tak poprostu iterować.
W teoriii C# pozwala nam na iterowanie po properties ale nie chcemy być zależni od kolejności pól w klasie, łatwo tu o błędy gdy ta kolejność się zmieni.
Moglibyśmy to zamienić na pętlę + switch ale to nie polepszy czytelności kodu, bo będziemy mieli switch który robi to samo i wygląda gorzej.
To jawne przypisanie to najlepsze co możemy zrobić w tej sytuacji.
Podobny switch pojawia się też podczas procedury "clean and correct". Możnaby je połączyć, ale wynik mógłby być mało czytelny, póki co zostawiam jak jest.

Niejawne switche też pojawiają się podczas sprawdzania importedObject.Type. Najlepszą opcją byłoby stworzenie nowych klas dziedziczących po imported object zależnie od typu, gdzie każdy implementowałby np metodę printującą. To może być przesada, więc zostawiam jak jest.


## Powtórzenia, niejednolitość, inne

Nie musimy używać tu pętli for, foreach jest bardziej zwięzłe.  

    for (int i = 0; i < importedLines.Count; i++)


 Interfejs IEnumerable nic nam nie daje przy użyciu go jako pole, wybierzmy jakąś konkretną kolekcję.
 Jedyne co dostajemy to dodatkowe rzutowanie.  
 
    ((List<ImportedObject>)ImportedObjects).Add(importedObject);


Wielokrotnie w kodzie mamy sprawdzenie czy importedObject są ze sobą spokrewnione.
Możemy te instrukcje warunkowe spiąć do osobnej metody.
Do tego mamy zmienne impObj i importedObject, które nic nie mówią, zamieniamy je na child i parent.

    if (impObj.ParentType == importedObject.Type)
        {
            if (impObj.ParentName == importedObject.Name)
            {
                importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
            }
        }

Zamiast sumowania możemy użyc operatora inkrementacji

    parent.NumberOfChildren = 1 + parent.NumberOfChildren;

Operacja poprawy danych wejściowych jest podobna dla każdego pola, możemy tą funkcjonalnośc przenieść do osobnej metody.

    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");

Streamreader nie jest zamknięty po użyciu, używamy using.

Możnaby użyyć Linq do przetwarzania danych. Linq ma głównie ułatwiać czytelność ale nasze funkcje są teraz dosyć proste więc damy radę bez.

Możemy usunąć niepotrzebne importy.





