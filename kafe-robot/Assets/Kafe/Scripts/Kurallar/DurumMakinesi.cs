// Ders 3.3 · Oyun Durumları — durumlar ve izinli geçişler bir tabloda.
using System.Collections.Generic;
using System.Linq;

namespace Kafe
{
    public enum OyunDurumu { Menu, Oyun, Teslim, Dokuldu }

    public class DurumMakinesi
    {
        public OyunDurumu Simdi { get; private set; } = OyunDurumu.Menu;
        public int Reddedilen { get; private set; }

        public static readonly Dictionary<OyunDurumu, OyunDurumu[]> Gecisler = new Dictionary<OyunDurumu, OyunDurumu[]>
        {
            [OyunDurumu.Menu] = new[] { OyunDurumu.Oyun },
            [OyunDurumu.Oyun] = new[] { OyunDurumu.Teslim, OyunDurumu.Dokuldu, OyunDurumu.Menu },
            [OyunDurumu.Teslim] = new[] { OyunDurumu.Menu },
            [OyunDurumu.Dokuldu] = new[] { OyunDurumu.Menu },
        };

        public static int GecisSayisi => Gecisler.Sum(g => g.Value.Length);

        public bool Gec(OyunDurumu yeni)
        {
            if (Gecisler[Simdi].Contains(yeni)) { Simdi = yeni; return true; }
            Reddedilen++;
            return false;
        }
    }
}
