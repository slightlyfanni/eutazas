using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace eutazas
{
    internal class Program
    {

        static List<Utas> Beolvasas(string fajlNev)
        {
            List<Utas> utasok = new List<Utas>();
            foreach (var sor in File.ReadAllLines(fajlNev))
            {
                string[] adatok = sor.Split(' ');
                utasok.Add(new Utas
                {
                    Megallo = int.Parse(adatok[0]),
                    FelszallasDatuma = DateTime.ParseExact(adatok[1], "yyyyMMdd-HHmm", null),
                    KartyaAzonosito = adatok[2],
                    JegyTipus = adatok[3],
                    JegyErvenyesseg = adatok[3] == "JGY" ? adatok[4] : DateTime.ParseExact(adatok[4], "yyyyMMdd", null).ToString("yyyy-MM-dd")
                });
            }
            return utasok;
        }

        static bool Ervenyes(Utas utas)
        {
            if (utas.JegyTipus == "JGY")
                return int.Parse(utas.JegyErvenyesseg) > 0;
            else
                return DateTime.Parse(utas.JegyErvenyesseg) >= utas.FelszallasDatuma.Date;
        }

        static int NapokSzama(DateTime datum1, string datum2Str)
        {
            DateTime datum2 = DateTime.Parse(datum2Str);
            return (datum2 - datum1.Date).Days;
        }
        static void Main(string[] args)
        {
            List<Utas> utasok = Beolvasas("utasadat.txt");

            
            Console.WriteLine("2. feladat");
            Console.WriteLine($"A buszra {utasok.Count} utas szeretett volna felszállni.");

            
            int elutasitottak = utasok.Count(utas => !Ervenyes(utas));
            Console.WriteLine("3. feladat");
            Console.WriteLine($"A buszvezetőnek {elutasitottak} utast kellett elutasítania.");

            
            Console.WriteLine("4. feladat");
            var legforgalmasabb = utasok.GroupBy(utas => utas.Megallo)
                                        .OrderByDescending(gr => gr.Count())
                                        .ThenBy(gr => gr.Key)
                                        .First();
            Console.WriteLine($"A legtöbb utas ({legforgalmasabb.Count()} fő) a {legforgalmasabb.Key}. megállóban próbált felszállni.");

            
            int kedvezmenyesek = utasok.Count(utas => new[] { "TAB", "NYB" }.Contains(utas.JegyTipus) && Ervenyes(utas));
            int ingyenesek = utasok.Count(utas => new[] { "NYP", "RVS", "GYK" }.Contains(utas.JegyTipus) && Ervenyes(utas));

            Console.WriteLine("5. feladat");
            Console.WriteLine($"Ingyenesen utazók száma: {ingyenesek} fő");
            Console.WriteLine($"Kedvezményesen utazók száma: {kedvezmenyesek} fő");

            
            Console.WriteLine("7. feladat");
            List<string> figyelmeztetesek = new List<string>();
            foreach (var utas in utasok)
            {
                if (utas.JegyTipus != "JGY" && Ervenyes(utas))
                {
                    if (NapokSzama(utas.FelszallasDatuma, utas.JegyErvenyesseg) <= 3)
                    {
                        figyelmeztetesek.Add($"{utas.KartyaAzonosito} {utas.JegyErvenyesseg}");
                    }
                }
            }
            File.WriteAllLines("figyelmeztetes.txt", figyelmeztetesek);
            Console.WriteLine("A figyelmeztetes.txt állomány létrejött.");
            Console.ReadKey();
        }
       
    }
}






