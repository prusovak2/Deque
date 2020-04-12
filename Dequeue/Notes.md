#`Deque<T>` Notes
`Deque<T>` je implementace datové struktury zvané double ended queue. Tato datová struktura umožňuje přidání či odebrání prvního a posledního ze svých prvků v amortizovaně konstantním čase. Dále implementuje .Net rozhraní `IList<T>` a umožňuje tedy přístup k libovolnému ze svých prvků v konstantním čase, enumeraci svých prvků atd. Odevzdávaná implementace obsahuje generickou třídu `Deque<T>`, reprezentující samotnou frontu, rozhraní `IDeque<T>` rozšiřující rozhraní `IList<T>` a třídu `ReverseView<T>` poskytující abstrakci nad `Deque<T>` a umožňující a daty v `Deque<T>` pracovat v reverzním pořadí.

##Rozhraní `IDeque<T>`
```c#
public interface IDeque<T> : IList<T>
{
    T First { get; }
    T Last { get;  }
    void AddHead(T item);
    T RemoveHead();
    T RemoveTail();
    IDeque<T> Reverse();
}
```

Vlastnosti `T First` a `T Last` umožňují nahlédnout, jaký je první a poslední prvek fronty. 

Metoda `Add()` z rozhraní `IList<T>` obvykle slouží k přidání prvku na konec kolekce. Zde jsem se rozhodla vyhnout vyžadování jinak pojmenované metody plnící stejnou funkci a proto v rozhraní není žádná metoda `AddTail()` , její funkci zastane zmiňovaná `Add()` z `IList<T>`

Metoda `Reverse()` by  měla vracet datový typ poskytují reverzní pohled na data ve frontě.

##Třída `Deque<T>`

###`Deque<T>` jako abstrakce nad `Data<T>`

Třída `Deque<T>` slouží pouze jako abstrakce nad samotnými daty, která jsou uložena ve třídě `Data<T>`, jež je naimplementována jako privátní vnořená třída ve třídě `Deque<T>`. Samotná data a přímé operace s nimi jsou tedy zapouzdřené v `Data<T>`, o které lze uvažovat jako o implementačním detailu třídy `Deque<T>` a uživatel k ní jako takové nemá žádný přímý přístup. `Deque<T>`, (resp. `ReverseView<T>` diskutovaná později) tedy pouze volají metody naimplementované na Datech a až ty skutečně přistupují k uloženým datům. Idea tohoto přístupu byla oddělit logiku přímé práce s daty, která musí brát v úvahu skutečnou strukturu uložení dat (v 2D array) a uživatelské rozhraní, jež už se o skutečné uložení dat nestará a zvesela předstírá, že jsou data uložena sekvenčně. Další předpokládanou výhodou byla snadnější implementace dalších uživatelských rozhraní, která mohou měnit logiku toho, jak se na "sekvenčně" uložená data díváme (zmiňovaný `ReverseView<T>`).

Třída `Deque<T>` samozřejmě implementuje rozhraní `IDeque<T>`.

### `Enumerator` a `ReverseView` jako abstrakce nad `Deque<T>`

Další privátní vnořenou třídou v `Deque<T>` je její `Enumerator`, jehož instanci vrací metoda `Deque<T>.GetEnumerator<T>()`. `Deque<T>` propojuje `Enumerator` se svými daty. `Enumerator` tedy nepřistupuje k datum přímo, ale k enumeraci využívá přístup k datům přes metody naimplementované na `Deque<T>`. Idea je, že každý případně později doimplementovaný Enumerator či View třída (podobná `ReverseView<T>`), které pozmění logiku, s níž je na data nahlíženo, by si měly vystačit s přístupem k datům přes `Deque<T>` metody a do vlastní implementace třídy `Data<T>` by už nezasvěceným programátorem nemělo být zasahováno. Další takové View třídy by tedy měly poskytovat jen další vrstvu abstrakce nad `Deque<T>` jako takovou.

###`Enumerator` a `version`

Důležitým (a jediným) kusem logiky naimplementovaným v samotné třídě `Deque<T>` je mechanismus, který zabraňuje modifikaci dat během enumerace. 

```c#
    public long version { get; private set; } = 0;
    public T this [int i]
    {
        get 
        {
            return this.data[i];
        }
        set 
        {
            this.data[i] = value;
            version++;
        }
    }
```

Vlastnost `version` je inkrementována pokaždé, když dojde k modifikaci dat ve frontě. Je naimplementována jako `long`, aby modifikací mohlo eventuálně proběhnou mnoho bez rizika přetečení.

V okamžiku vytvoření nové instance Enumerátoru si tato instance uloží aktuální verzi (míněno hodnotu `version` vlastnosti) `Deque<T>`, kterou enumeruje. Každé volání metody `MoveNext()` či getteru vlastnosti `Current` nejprve zkontroluje, zda se verze fronty z okamžiku vzniku Enumerátoru shoduje s aktuální verzí enumerované fronty a pokud nikoli, je vyhozena `InvalidOperationException`.

```c#
        public S Current { 
            get
            {
                if (curIndex<0 || curIndex >= this.Que.Count ||
                    this.version != Que.version)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return this.Que[curIndex];
                }
            } 
        }
```

`InvalidOperationException` je vyhozena i při pokusu o volání `Current` před prvním zavoláním `MoveNext()` (`curIndex` je při vytvoření `Enumeratoru` inicializován na -1), či po "posledním" volání `MoveNext()`, tedy toho, co vrací `false`, neboť ve frontě už není žádný další prvek, ne který by bylo možno přistoupit.

### `Data<T>`

Samotná data jsou ve třídě `Data<T>` uložena v blocích po 128 prvcích (volba tohoto čísla je vcelku náhodná), sdružených do pole referencí na pole nazvaného `data`.

```c#
private S[][] data = new S[2][];
```

Indexer na `Data<T>` se spolu s metodami `GetIndexOfBlock()` a `GetIndexInBlock()` stará o to, aby se data tvářila, že jsou uložena sekvenčně a nikoli v 2D poli.

```c#
private int GetIndexOfBlock(int i) => ((i) / sizeOfBlock);
private int GetIndexInBlock(int i) => ((i) % sizeOfBlock);
```

Aby byl možný amortizovaně konstantní přístup jak na začátek tak na konec fronty, je prostor vyhrazený k uložení dat plněn "odprostřed". Tedy při inicializaci či případném zvětšování pole je snaha o to, aby před prvním a za posledním prvkem pole bylo stejné množství volných pozic k uložení dalších prvků. Index první obsazené pozice relativně ke skutečnému začátku fyzického uložiště (vnímaného sekvenčně, nikoli jako 2D pole) je uložen ve vlastnosti `HeadIndex`. Indexer na `Data<T>` tuto hodnotu bere v úvahu a při indexování ji přičte ke každému jemu předanému indexu, takže i v rámci třídy `Data<T>` je možné (a nutné) přistupovat k prvnímu prvku fronty způsobem `Data[0]` a nikoli `Data[HeadIndex]`.

 

####Paměťová optimalizace při zvětšování uložiště

Zde je na místě zmínit důležitý trik, který, jak doufám, může snížit paměťovou náročnost mé `Deque<T>`. Je-li do dat přidáván nový prvek, je třeba samozřejmě nejprve zkontrolovat, že na příslušném konci fronty ještě volné místo v uložišti. Pokud není, je nutné zdvojnásobit velikost pole referencí na bloky, zkopírovat stávající data "doprostřed" nového uložiště a naalokovat bloky nové.
Má optimalizace spočívá v tom, že metoda `DoubleSize()`, zodpovědná za zdvojnásobení velikosti pole referencí na bloky a překopírování stávajících referencí do nově vzniklého pole, sama o sobě žádné nové bloky nealokuje. Pole referencí na bloky tedy po zavolání obsahuje ve své druhé a třetí čtvrtině stávající data, zatímco první a čtvrtá čtvrtina obsahuje pouze `null` reference.

Metoda `DoubleSize()` je volána výhradně z metody `AlloclockBeginning()` či `AllocBlockEnd()`. Ty zkontrolují, zda je za příslušném konci pole referencí ještě místo na novou referenci na blok. Pokud není, zavolají `DoubleSize()`. Následně naalokují právě 1 blok a referenci na něj uloží na příslušný konec pole referencí.

Výhodou tohoto přístupu je, že existuje nejvýš jeden nezaplněný naalokovaný blok na začátku a jeden na konci fronty. Po zdvojnásobení pole referencí tedy nevzniká dvojnásobný overhead naalokovaného místa proti skutečně uloženým datům, jaký by vznikal, kdyby `DoubleSize()` skutečně naalokovala všechny bloky, na které bude nové pole poskytovat reference. Vzniká overhead maximálně 127 nových neobsazených pozic.

Je důležité si uvědomit, že tento přístup nepodkopává amortizovaně konstantní složitost přidání prvku na začátek či konec fronty, neboť časově náročné kopírování do nového pole referencí je prováděno stejně často jako bez této optimalizace. Pouze alokace nových bloků (která by stejně musela být provedena), není uskutečněna najedou, ale je rozložena do více okamžiků v průběhu algoritmu.  

Nevýhodou  přístupu je složitější implementace náchylnější k bugům.

####`Clear()`

Tímto jsme pokryli implementaci indexeru a logiku přidávání prvku na začátek a konec fronty (metody `AddBeginning()` a `AddEnd()` ve třídě `Data<T>`). Implementace dalších metod třídy `Data<T>` typicky staví na těchto metodách a není jinak ničím pozoruhodná. Jedinou výjimkou kterou ještě zmíním je metoda `Clear()`, u které mi nebylo jasné, jestli po odstranění všech prvků z fronty dává smysl zmenšit velikost uložiště zpět na počáteční dva nanalokované bloky a šetřit tak paměť. Nakonec jsem se rozhodla si již naalokovanou paměť ponechat v rámci úspory času, který by GC strávil jejím uvolňováním a následnou alokací paměti nové, chtěl-li by uživatel po vyčištění fronty do ní  přidat srovnatelné množství dalších prvků.

## Třída`ReversedView<T>`

Jak již bylo zmíněno, tato třída poskytuje abstrakci nad `Deque<T>` a umožňuje její prvky procházet v reverzním pořadí. Činí tak pouze s přístupem k API `Deque<T>` a nikoli k datům samotným. `ReverseView<T>` nedisponuje vlastní kopií dat. Jedná se pouze o jakýsi wrapper kolem konkrétní instance `Deque<T>`, na níž si `ReversedView<T>` udržuje referenci ve vlastnosti `deque`. Modifikace provedené přes `ReversedView<T>` tedy modifikují původní `Deque<T>` včetně inkrementace její `version`. Třída `ReversedView<T>` implementuje rozhraní `IDeque<T>` a je tedy možné s ní pracovat, jako by se jednalo o frontu jako takovou. Metoda `Reverse()` na třídě `ReversedView<T>` vrací instanci `Deque<T>`, kterou obaluje. Metoda `Reverse()` na třídě `Deque<T>` vrací novou instanci `ReversedView<T>`.



`ReversedView<T>` disponuje vlastním enumerátorem, jež ji prochází reverzně.





