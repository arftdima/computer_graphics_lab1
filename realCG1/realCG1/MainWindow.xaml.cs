using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

/*
 
    для изменения нужно вначале сделать радиобаттон

    если выбран rgb - всё пересчитывается и меняется цвет 
    если выбран hsv - пересчитывается только rgb   (доделать цвет)
    если выбран cmyk - ничего (сделать всё, но это однообразно и нефиг делать, так что я не делал)
    если выбран luv - ничего (сделать всё, но это однообразно и нефиг делать, так что я не делал)


     */
namespace realCG1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte _R = 0;
        private byte _G = 0;
        private byte _B = 0;

        private int _H = 0;
        private double _S = 0.0;
        private double _V = 0.0;

        private double _C = 0.0;
        private double _M = 0.0;
        private double _Y = 0.0;
        private double _K = 0.0;

        private double _L = 0.0;
        private double _U = 0.0;
        private double _LV = 0.0;

        private Boolean brgbch = false;
        private Boolean bhsvch = false;
        private Boolean bcmykch = false;
        private Boolean bluvch = false;


        public MainWindow()
        {
            InitializeComponent();

            lr.Content = 0;
            lg.Content = 0;
            lb.Content = 0;
            lh.Content = 0.ToString() + "°";
            ls.Content = 0.ToString() + "%";
            lv.Content = 0.ToString() + "%";


            changeBackgroundColorRGB();
        }

        private void srvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lr.Content = (int)s.Value;
            _R = (byte)s.Value;
            if(brgbch) changeBackgroundColorRGB();

        }

        private void sgvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lg.Content = (int)s.Value;
            _G = (byte)s.Value;
            if (brgbch) changeBackgroundColorRGB();

        }

        private void sbvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lb.Content = (int)s.Value;
            _B = (byte)s.Value;
            if (brgbch) changeBackgroundColorRGB();

        }

        private void changeBackgroundColorRGB()
        {  
            var a = new SolidColorBrush();
            a.Color = System.Windows.Media.Color.FromRgb(_R, _G, _B);
            Background = a;

            var brash = new SolidColorBrush();
            brash.Color = System.Windows.Media.Color.FromRgb((byte)(255 - _R), (byte)(255 - _B), (byte)(255 - _G));
            lrc.Background = brash;
            lgc.Background = brash;
            lbc.Background = brash;
            lhc.Background = brash;
            lsc.Background = brash;
            lvc.Background = brash;
            lcc.Background = brash;
            lmc.Background = brash;
            lyc.Background = brash;
            lkc.Background = brash;
            llc.Background = brash;
            luc.Background = brash;
            llvc.Background = brash;

            #region HSV         
            double _rn = _R / 255d;
            double _gn = _G / 255d;
            double _bn = _B / 255d;

            double max = _rn;
            double min = _rn;

            if (max < _gn) max = _gn;
            if (max < _bn) max = _bn;

            if (min > _gn) min = _gn;
            if (min > _bn) min = _bn;

            _V = max;
            _S = max == 0 ? 0 : 1d - min / max;

            if (max == min) _H = 0;
            else if (max == _rn && _gn >= _bn) _H = (int)(0d + 60 * (_gn - _bn) / (max - min));
            else if (max == _rn && _gn < _bn) _H = (int)(360d + 60 * (_gn - _bn) / (max - min));
            else if (max == _gn) _H = (int)(120d + 60 * (_bn - _rn) / (max - min));
            else _H = (int)(240d + 60 * (_rn - _gn) / (max - min));


            sh.Value = _H;
            ss.Value = _S;
            sv.Value = _V;

            #endregion

            #region CMYK

            double k, c, m, y, r = _R, g = _G, b = _B;

            k = Math.Min(1 - r / 255.0, Math.Min(1 - g / 255.0, 1 - b / 255.0));

            if (k == 1)
            {
                sc.Value = 0;
                sm.Value = 0;
                sy.Value = 0;
                sk.Value = 1;
            }
            else
            {
                c = (1 - r / 255.0 - k) / (1 - k);
                m = (1 - g / 255.0 - k) / (1 - k);
                y = (1 - b / 255.0 - k) / (1 - k);

                _C = c;
                _M = m;
                _Y = y;
                _K = k;



                sc.Value = _C;
                sm.Value = _M;
                sy.Value = _Y;
                sk.Value = _K;
            }

            #endregion

            #region Luv

            Double F_rgb_xyz(Double ans)
                   => ans >= 0.04045 ? Math.Pow((ans + 0.055) / 1.055, 2.4) : ans / 12.92;

            Double Rn, Gn, Bn;

            Rn = F_rgb_xyz(r / 255.0) * 100;
            Gn = F_rgb_xyz(g / 255.0) * 100;
            Bn = F_rgb_xyz(b / 255.0) * 100;

            Double X, Y, Z;

            if (Rn == 0 && Gn == 0 && Bn == 0)
                Rn += 0.000001;

            X = Rn * 0.412453 + Gn * 0.357580 + Bn * 0.180423;
            Y = Rn * 0.212671 + Gn * 0.715160 + Bn * 0.072169;
            Z = Rn * 0.019334 + Gn * 0.119193 + Bn * 0.950227;

            Double u_, v_, u_w_, v_w_;

            u_ = 4 * X / (X + 15 * Y + 3 * Z);
            v_ = 9 * Y / (X + 15 * Y + 3 * Z);

            Double X_w = 95.047, Y_w = 100.0, Z_w = 108.883;

            u_w_ = 4 * X_w / (X_w + 15 * Y_w + 3 * Z_w);
            v_w_ = 9 * Y_w / (X_w + 15 * Y_w + 3 * Z_w);

            Double F_xyz_Luv(Double ans)
                => ans >= 0.008856 ? Math.Pow(ans, 1 / 3.0) : 7.787 * ans + 16 / 116.0;

            Double L, u, _v;

            L = 116 * F_xyz_Luv(Y / Y_w) - 16;
            u = 13 * L * (u_ - u_w_);
            _v = 13 * L * (v_ - v_w_);


            _L = L;
            _U = u;
            _LV = _v;

            sl.Value = _L;
            su.Value = _U;
            slv.Value = _LV;

            #endregion


        }

        private void brgb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _R = Byte.Parse(tr.Text);
                _G = Byte.Parse(tg.Text);
                _B = Byte.Parse(tb.Text);
            }
            catch (Exception ex)
            {
                //
            }
            finally
            {
                // changeBackgroundColorRGB();
                sr.Value = _R;
                sg.Value = _G;
                sb.Value = _B;
            }
        }


        private void shvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lh.Content = ((int)s.Value).ToString() + "°";
            _H = (int)s.Value;
            if (bhsvch) changeBackgroundColorHSV();
        }

        private void ssvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            ls.Content = ((int)((s.Value * 100))).ToString() + "%";
            _S = s.Value;
            if (bhsvch) changeBackgroundColorHSV();
        }
    
        private void svvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lv.Content = ((int)((s.Value * 100))).ToString() + "%";
            _V = s.Value;
            if (bhsvch) changeBackgroundColorHSV();
        }

        private void changeBackgroundColorHSV()
        {
            double _hn = _H;
            double _sn = _S * 100;
            double _vn = _V * 100;

            int _hi = (int)((_hn / 60) % 6);
            int v_min = (int)((100 - _sn) * _vn / 100);
            double a = (_vn - v_min) * ((_hn % 60) / 60);
            int v_inc = (int)(v_min + a);
            int v_dec = (int)(_vn - a);

            switch (_hi)
            {
                case 0:
                    _R = (byte)(_vn * 255 / 100);
                    _G = (byte)v_inc;
                    _B = (byte)v_min;
                    break;
                case 1:
                    _R = (byte)v_dec;
                    _G = (byte)(_vn * 255 / 100);
                    _B = (byte)v_min;
                    break;
                case 2:
                    _R = (byte)v_min;
                    _G = (byte)(_vn * 255 / 100);
                    _B = (byte)v_inc;
                    break;
                case 3:
                    _R = (byte)v_min;
                    _G = (byte)v_dec;
                    _B = (byte)(_vn * 255 / 100);
                    break;
                case 4:
                    _R = (byte)v_inc;
                    _G = (byte)v_min;
                    _B = (byte)(_vn * 255 / 100);
                    break;
                case 5:
                    _R = (byte)(_vn * 255 / 100);
                    _G = (byte)v_min;
                    _B = (byte)v_dec;
                    break;
            }

            sr.Value = _R;
            sg.Value = _G;
            sb.Value = _B;
        }


        private void scvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lc.Content = ((int)((s.Value * 100))).ToString() + "%";
            _C = (int)s.Value;
        }

        private void smvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lm.Content = ((int)((s.Value * 100))).ToString() + "%";
            _M = (int)s.Value;
        }

        private void syvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            ly.Content = ((int)((s.Value * 100))).ToString() + "%";
            _Y = (int)s.Value;
        }

        private void skvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lk.Content = ((int)((s.Value * 100))).ToString() + "%";
            _K = (int)s.Value;
        }

        private void changeBackgroundColorCMYK()
        {

        }


        private void slvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            ll.Content = (int)s.Value;
            _L = (int)s.Value;
        }

        private void suvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            lu.Content = (int)s.Value;
            _U = (int)s.Value;
        }

        private void slvvchAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            llv.Content = (int)s.Value;
            _LV = (int)s.Value;
        }

        private void changeBackgroundColorLuv()
        {

        }


        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            var md_color = colorPicker.SelectedColor;
            
            _R = (byte)(md_color?.R);
            _G = (byte)(md_color?.G);
            _B = (byte)(md_color?.B);

            sr.Value = _R;
            sg.Value = _G;
            sb.Value = _B;

        }


        private void rbrgbCheck(object sender, RoutedEventArgs e)
        {
            brgbch = true;
            bhsvch = false;
            bcmykch = false;
            bluvch = false;
        }
        private void rbrgbUncheck(object sender, RoutedEventArgs e)
        {}

        private void rbhsvCheck(object sender, RoutedEventArgs e)
        {
            brgbch = false;
            bhsvch = true;
            bcmykch = false;
            bluvch = false;
        }
        private void rbhsvUncheck(object sender, RoutedEventArgs e)
        {}

        private void rbcmykCheck(object sender, RoutedEventArgs e)
        {
            brgbch = false;
            bhsvch = false;
            bcmykch = true;
            bluvch = false;
        }
        private void rbcmykUncheck(object sender, RoutedEventArgs e)
        {}

        private void rbluvCheck(object sender, RoutedEventArgs e)
        {
            brgbch = false;
            bhsvch = false;
            bcmykch = false;
            bluvch = true;
        }
        private void rbluvUncheck(object sender, RoutedEventArgs e)
        {}

    }
}
