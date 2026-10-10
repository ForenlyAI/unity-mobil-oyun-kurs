// Ders 2.3 · Bardağın Dökülmesi — benzetim kaydından bardağın yatay hızı ve ivmesi.
// Kayıt 50 Hz (0,02 s). Gürültüyü azaltmak için 0,5 s'lik pencereyle düzlenir.
using System;
using System.Globalization;
using System.Linq;

namespace Kafe
{
    public class KayitSonucu
    {
        public int ornek; public double sure, yol, enBuyukHiz, enBuyukIvme; public int esikUstu;
        public double[] zaman, hiz, ivme;
    }

    public static class KayitAnalizi
    {
        public static double[] Duzle(double[] d, int w)
        {
            var o = new double[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                int a = Math.Max(0, i - w), b = Math.Min(d.Length - 1, i + w);
                double t = 0; for (int k = a; k <= b; k++) t += d[k];
                o[i] = t / (b - a + 1);
            }
            return o;
        }

        public static KayitSonucu Coz(string csv, double esik, double dt = 0.02, int pencere = 12)
        {
            var satir = csv.Split('\n').Skip(1).Where(l => l.Trim().Length > 0)
                .Select(l => l.Split(',').Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray()).ToArray();
            int n = satir.Length;
            double[] t = satir.Select(a => a[0]).ToArray(), x = satir.Select(a => a[6]).ToArray(), y = satir.Select(a => a[7]).ToArray();
            var xs = Duzle(x, pencere); var ys = Duzle(y, pencere);
            var r = new KayitSonucu { ornek = n, sure = t[n - 1], zaman = t, hiz = new double[n], ivme = new double[n] };
            for (int i = 1; i < n; i++) r.yol += Math.Sqrt(Math.Pow(x[i] - x[i - 1], 2) + Math.Pow(y[i] - y[i - 1], 2));
            var vx = new double[n]; var vy = new double[n];
            for (int i = 1; i < n - 1; i++)
            {
                vx[i] = (xs[i + 1] - xs[i - 1]) / (2 * dt); vy[i] = (ys[i + 1] - ys[i - 1]) / (2 * dt);
                r.hiz[i] = Math.Sqrt(vx[i] * vx[i] + vy[i] * vy[i]); r.enBuyukHiz = Math.Max(r.enBuyukHiz, r.hiz[i]);
            }
            var vxs = Duzle(vx, pencere); var vys = Duzle(vy, pencere);
            for (int i = 1; i < n - 1; i++)
            {
                r.ivme[i] = Math.Sqrt(Math.Pow((vxs[i + 1] - vxs[i - 1]) / (2 * dt), 2) + Math.Pow((vys[i + 1] - vys[i - 1]) / (2 * dt), 2));
                r.enBuyukIvme = Math.Max(r.enBuyukIvme, r.ivme[i]);
                if (r.ivme[i] > esik) r.esikUstu++;
            }
            return r;
        }
    }
}
