
using MailAppMAUI.General;
using MailAppMAUI.Config;

namespace MailAppMAUI.General
{
    public enum TipoNorm { None, Left, Right };

    public class Norm
    {
        static Configuration Conf { get; set; }

        public Norm() 
        {
            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }
        }

        static TipoNorm m_TipArtic;

        public static TipoNorm TipArtic
        {
            get
            {
                if (m_TipArtic == TipoNorm.None)
                    m_TipArtic = TipoNorm.Left;

                return m_TipArtic;
            }
            set { m_TipArtic = value; }
        }

        static Norm()
        {
        }

        #region NORMALIZACION DE CODIGOS ENUMERADOS

        public static string GetCodigo(string Codigo, int LonCod, int LonSer)
        {
            var nNorm = TipoNorm.Left;

            return GetCodigo(Codigo, LonCod, LonSer, nNorm);
        }


        /// <summary> Normaliza un codigo respetando la longitud de serie
        /// La serie debe estar incluida y se normaliza el resto del codigo 
        /// </summary>
        /// <param name="Codigo"> Codigo base a normalizar     </param>
        /// <param name="config"> Enumerado de configuracion   </param>
        /// <param name="LonSer"> Longitud de la serie inicial </param>
        /// <param name="nNorm">  Tipo de normalizacion        </param>
        /// <returns> Codigo normalizado con la serie </returns>

        public static string GetCodigo(string Codigo, int LonCod, int LonSer,
                                       TipoNorm nNorm = TipoNorm.None)
        {
            if (Codigo == null)
                return null;

            Codigo = Codigo.TrimEnd();

            if (LonCod != Codigo.Length)
            {
                if (LonCod < Codigo.Length && LonSer == 0)
                    Codigo = Codigo.Substring(0, LonCod);
                else
                {
                    string serie, orden;

                    switch (nNorm)
                    {
                        case TipoNorm.Left:
                            if (LonSer == 0)
                                Codigo = Codigo.PadLeft(LonCod, '0');
                            else
                            {
                                serie = Codigo.Substring(0, LonSer);
                                orden = Codigo.Substring(LonSer);
                                int difer = orden.Length - (LonCod - LonSer);
                                if (difer > 0)
                                    orden = orden.Substring(difer);

                                Codigo = serie + orden.PadLeft(LonCod - LonSer, '0');
                            }
                            break;

                        case TipoNorm.Right:
                            if (LonSer == 0)
                                Codigo = Codigo.PadRight(LonCod, '0');
                            else
                            {
                                serie = Codigo.Substring(0, LonSer);
                                orden = Codigo.Substring(LonSer);
                                int difer = orden.Length - (LonCod - LonSer);
                                if (difer > 0)
                                    orden = orden.Substring(difer);

                                Codigo = serie + orden.PadRight(LonCod - LonSer, '0');

                                // Codigo = Codigo.Substring(0, LonSer) +
                                //          Codigo.Substring(LonSer).PadRight(LonCod - LonSer, '0');
                            }
                            break;
                    }
                }
            }

            return Codigo;
        }

        /// <summary> Normaliza un codigo con la serie inicial dada
        /// La serie se añade al codigo y se ajusta la longitud total
        /// </summary> 
        /// <param name="Codigo"> Codigo base a normalizar   </param>
        /// <param name="config"> Enumerado de configuracion </param>
        /// <param name="Serie">  Serie a añadir al codigo   </param>
        /// <returns> Codigo normalizado con la serie </returns>

        public static string GetCodigo(string Codigo, int LonCod, string Serie)
        {
            if (Str.Empty(Codigo))
                return string.Empty;

            Codigo = Codigo.Trim();

            int LonSer = 0;
            int LonTot = Codigo.Length;

            if (LonCod == 0)
                LonCod = LonTot;

            if (!Str.Empty(Serie))
                LonSer = Serie.Length;

            Codigo = Codigo.ToUpper();

            if (LonTot == LonCod)
            {
                if (LonSer == 0 || Codigo.StartsWith(Serie))
                    return Codigo;
            }
            else
            {
                if (LonTot > LonCod + LonSer)
                    Codigo = Codigo.Substring(LonTot - LonCod + LonSer);
            }

            TipoNorm nNorm = TipoNorm.Left;

            // Normalizar siemrpoe a ioquierda hasqeu se use InterData
            // 
            // if (Enum.Equals(config, Config.Artic.Longitud))
            // {
            //    nNorm = (TipoNorm)Config.GetInt(Config.Norm.Tipo);
            //    Error: nNorm = (TipoNorm)Config.Norm.Tipo;
            // }

            switch (nNorm)
            {
                case TipoNorm.Left:
                    if (LonSer == 0)
                        Codigo = Codigo.PadLeft(LonCod, '0');
                    else
                        Codigo = Serie + Codigo.PadLeft(LonCod - LonSer, '0');
                    break;

                case TipoNorm.Right:
                    if (LonSer == 0)
                        Codigo = Codigo.PadRight(LonCod, '0');
                    else
                        Codigo = Serie + Codigo.PadRight(LonCod - LonSer, '0');
                    break;
            }

            return Codigo;
        }

        /// <summary> Incrementa el codigo dado respetando la serie
        /// </summary>
        /// <param name="Codigo"> Codigo base a normalizar  </param>
        /// <param name="config"> Enumerado de configuracion </param>
        /// <param name="Serie">  Serie a añadir al codigo  </param>
        /// <param name="Difer">  Valor a usar como incremento </param>
        /// <returns></returns>

        public static string NextCode(string Codigo, int LonCod, string Serie, int difer)
        {
            int LonSer = 0;

            if (!Str.Empty(Serie))
                LonSer = Serie.Length;

            if (!Str.Empty(Codigo))
            {
                Codigo = Str.Substring(Codigo, LonSer).TrimEnd();
                Codigo = NextString(Codigo, 0, difer);
            }

            if (LonCod > LonSer)
            {
                //if (Enum.Equals(config, Config.Artic.Longitud))
                //{
                //    if ((TipoNorm)Config.GetInt(Conf.App.Norm.Tipo) != TipoNorm.Left)
                //        LonCod = 0;
                //}

                if (LonCod > 0)
                    Codigo = Codigo.PadLeft(LonCod - LonSer, '0');
            }

            if (LonSer > 0)
                Codigo = Serie + Codigo;

            return Codigo;
        }

        /// <summary> Incrementa el codigo dado respetando la serie
        /// </summary>
        /// <param name="Codigo"> Codigo base a normalizar   </param>
        /// <param name="config"> Enumerado de configuracion </param>
        /// <param name="Serie">  Serie a añadir al codigo   </param>
        /// <returns></returns>

        public static string NextCode(string Codigo, int LonCod, string Serie)
        {
            return NextCode(Codigo, LonCod, Serie, 1);
        }

        public static string NextCode(string codigo, int lonCod)
        {
            if (lonCod < 0 && codigo != null)
                lonCod = codigo.Trim().Length;

            return NextCode(codigo, lonCod, 1);
        }

        public static string NextCode(int codigo, int lonCod, int difer)
        {
            return NormCode((codigo + difer).ToString(), lonCod);
        }

        public static string NextCode(int codigo, int lonCod)
        {
            return NormCode((codigo + 1).ToString(), lonCod);
        }

        public static string NextCode(string codigo, int loncod, int difer)
        {
            if (loncod == 0 && codigo != null)
                loncod = codigo.Trim().Length;

            codigo = NextString(codigo, loncod, difer);

            return codigo;
        }

        /// <summary> Metodo general para incrementar una cadena de texto
        /// Busca e incrementa la parte numerica empezando por el final
        /// Si se indica una longitud no nula se normaliza el resultado
        /// Si la longitud pasada es nula se mantiene la longitud actual
        /// </summary>
        /// <param name="text">   Cadena a incrementar    </param>
        /// <param name="loncod"> Longitud final opcional </param>
        /// <param name="difer">  Incremento de la cadena </param>
        /// <returns> Cadena incrementada y normalizada   </returns>

        public static string NextString(string text, int loncod, int difer)
        {
            if (text == null)
                text = string.Empty;

            try
            {
                int length = text.Length;
                int value = 0;
                int index1 = 0;
                int index2 = -1;

                bool digit = false;

                if (length > 0)
                {
                    for (int index = length - 1; index >= 0; index--)
                    {
                        if (char.IsDigit(text[index]))
                        {
                            if (!digit)
                            {
                                digit = true;

                                if (index < length - 1)
                                    index2 = index + 1;
                            }
                        }
                        else
                        {
                            if (digit)
                            {
                                index1 = index + 1;
                                break;
                            }
                        }
                    }

                    if (index2 > 0)
                        value = Data.ToInt(text.Substring(index1, index2 - index1));
                    // value = Int32.Parse(text.Substring(index1, index2 - index1));
                    else
                    {
                        if (digit)
                            value = Data.ToInt(text.Substring(index1));
                        // value = Int32.Parse(text.Substring(index1));
                        else
                            index1 = length;
                    }
                }

                value += difer;

                string number = value.ToString();

                if (loncod > 0)
                {
                    if (loncod < text.Length && !digit)
                    {
                        index1 = loncod;
                        number = "";
                    }
                    else
                    {
                        int offset = number.Length - loncod + index1;

                        if (offset > 0 && offset <= number.Length)
                            number = number.Substring(offset);
                        else
                        {
                            if (loncod >= index1)
                            {
                                int nlen = loncod;

                                if (index2 > 0 && index2 < loncod)
                                    nlen = index2;

                                number = number.PadLeft(nlen - index1, '0');
                                // number = number.PadLeft(loncod - index1, '0');

                            }
                        }
                    }
                }

                if (index2 > 0)
                    text = text.Substring(0, index1) + number + text.Substring(index2);
                else
                    text = text.Substring(0, index1) + number;

            }
            catch (Exception exc)
            {
            }

            return text;
        }

        /// <summary> Incrementa la parte numerica de una cadena de texto 
        /// Este metodo no normaliza la cadena de texto resultante
        /// </summary>
        /// <param name="text">   Cadena a incrementar    </param>
        /// <returns> Cadena incrementada </returns>

        public static string NextString(string text)
        {
            if (text != null)
                text = NextString(text, 0, 1);
            else
                text = "1";

            return text;
        }

        /// <summary> Normalizacion de codigos segun la longitud dada
        /// Si se indica una longitud no nula se normaliza el resultado
        /// Si la longitud pasada es nula se mantiene la longitud actual
        /// Si no comienza por un digito se normaliza la parte numerica
        /// </summary>
        /// <param name="codigo"> Cadena a normalizar </param>
        /// <param name="lonCod"> Longitud del codigo </param>
        /// <returns> Codigo normalizado </returns>

        public static string NormCode(string codigo, int lonCod)
        {
            if (codigo != null)
                codigo = codigo.Trim();
            else
                codigo = string.Empty;

            if (lonCod == 0)
                lonCod = codigo.Length;

            if (codigo.Length > 0 && !char.IsDigit(codigo[0]))
                codigo = NextCode(codigo, lonCod, 0);
            else
                codigo = codigo.PadLeft(lonCod, '0');

            return codigo;
        }

        /// <summary> Normalizacion de codigos numericos a una longitud
        /// </summary>
        /// <param name="codigo"> Numero a normalizar </param>
        /// <param name="lonCod"> Longitud del codigo </param>
        /// <returns> Codigo normalizado </returns>

        public static string NormCode(int codigo, int lonCod)
        {
            return NormCode(codigo.ToString(), lonCod);
        }

        #endregion

        #region NORMALIZACION DE CODIGOS (COMPATIBILIDAD)

        /// <summary> Normaliza un codigo respetando la longitud de serie
        /// La serie debe estar incluida y se normaliza el resto del codigo 
        /// </summary>
        /// <param name="Codigo"> Codigo base a normalizar     </param>
        /// <param name="IdVar">  Identificador de variable    </param>
        /// <param name="LonSer"> Longitud de la serie inicial </param>
        /// <returns> Codigo normalizado con la serie </returns>

        [Obsolete]
        public static string GetCodigo(string Codigo, string IdVar, int LonSer)
        {
            if (Codigo == null)
                return null;

            Codigo = Codigo.TrimEnd();

            AppNorm.SetName(IdVar, "Long");

            int LonCod = Conf.App.Docum.Longitud;

            if (LonCod != Codigo.Length)
            {
                TipoNorm nNorm = TipoNorm.Left;
                if (IdVar == "Artic_Codigo")
                    nNorm = TipArtic;

                switch (nNorm)
                {
                    case TipoNorm.Left:
                        if (LonSer == 0)
                            Codigo = Codigo.PadLeft(LonCod, '0');
                        else
                            Codigo = Codigo.Substring(0, LonSer) +
                                     Codigo.Substring(LonSer).PadLeft(LonCod - LonSer, '0');
                        break;

                    case TipoNorm.Right:
                        if (LonSer == 0)
                            Codigo = Codigo.PadRight(LonCod, '0');
                        else
                            Codigo = Codigo.Substring(0, LonSer) +
                                     Codigo.Substring(LonSer).PadRight(LonCod - LonSer, '0');
                        break;
                }
            }

            return Codigo;
        }

        /// <summary> Incrementa el codigo dado respetando la serie
        /// </summary>
        /// <param name="Codigo"> Codigo base a normalizar  </param>
        /// <param name="IdVar">  Identificador de variable </param>
        /// <param name="Serie">  Serie a añadir al codigo  </param>
        /// <returns></returns>

        [Obsolete]
        public static string NextCode(string Codigo, string IdVar, string Serie)
        {
            return NextCode(Codigo, IdVar, Serie, 1);
        }

        /// <summary> Incrementa el codigo dado respetando la serie
        /// </summary>
        /// <param name="Codigo"> Codigo base a normalizar  </param>
        /// <param name="IdVar">  Identificador de variable </param>
        /// <param name="Serie">  Serie a añadir al codigo  </param>
        /// <param name="Difer">  Valor a usar como incremento </param>
        /// <returns></returns>

        [Obsolete]
        public static string NextCode(string Codigo, string IdVar, string Serie, int difer)
        {
            int LonSer = 0;
            int Value = 0;
            int LonCod = GetLong(IdVar);

            if (!Str.Empty(Serie))
                LonSer = Serie.Length;

            if (!Str.Empty(Codigo))
            {
                Codigo = Str.Substring(Codigo, LonSer).TrimEnd();

                if (!Str.Empty(Codigo))
                {
                    if (char.IsDigit(Codigo[0]))
                        Value = (int)Data.ToValue(Codigo, TypeCode.Int32);
                    else
                        return "".PadRight(LonCod, ' ');
                }
            }

            Codigo = (Value + difer).ToString();

            if (LonCod > LonSer)
            {
                if (IdVar != "Artic_Codigo" || TipArtic != TipoNorm.None)
                    Codigo = Codigo.PadLeft(LonCod - LonSer, '0');
            }

            if (LonSer > 0)
                Codigo = Serie + Codigo;

            return Codigo;
        }

        [Obsolete]
        private static int GetLong(string Clave)
        {
            return 0;
            //string LonClv = AppNorm.SetName(Clave, "Longitud");
            //int LonVar = Config.GetInt(LonClv);

            //if (LonVar == 0)
            //    LonVar = Config.GetInt("Longitud", "Docum");

            //return LonVar;
        }

        #endregion 

        #region NORMALIZACION DE CODIGOS ESPECIFICOS

        public static string GetDocum(string Codigo, string Serie)
        {
            return GetCodigo(Codigo, Conf.App.Docum.Longitud, Serie);
        }

        public static string GetDocum(long docum, string Serie)
        {
            return GetCodigo(docum.ToString(), Conf.App.Docum.Longitud, Serie);
        }

        public static string GetNumDoc(string Codigo)
        {
            if (Str.Empty(Codigo))
                return "";

            int LonSer = Conf.App.Docum.LonSerie;

            if (LonSer == 0)
                LonSer = 2;

            if (Codigo.Length > LonSer)
                Codigo = Codigo.Substring(LonSer);

            return Codigo;
        }

        public static string GetDocum(string Codigo)
        {
            int LonSer = Conf.App.Docum.LonSerie;

            Codigo = Codigo.Trim();
            if (Codigo.Length <= LonSer)
                Codigo = GetSerie(Codigo);
            else
                Codigo = GetCodigo(Codigo, Conf.App.Docum.Longitud, LonSer);

            return Codigo;
        }

        public static string GetDocum(ref string codigo, ref string serie)
        {
            int LonSer = Conf.App.Docum.LonSerie;
            // var xx = Config.GetInt(Config.Docum.Longitud);

            string docum;

            if (LonSer == 0)
                LonSer = 2;

            codigo = codigo.Trim();
            if (serie == null)
                serie = GetSerie(codigo);

            if (codigo.Length <= LonSer)
            {
                serie = GetSerie(codigo);
                codigo = "";
                docum = serie;
            }
            else
            {
                int londoc = Conf.App.Docum.Longitud;
                docum = GetCodigo(serie + codigo, londoc, LonSer);
                codigo = docum.Substring(serie.Length);
            }

            return docum;
        }


        public static string GetClien(string Codigo)
        {
            return GetCodigo(Codigo, Conf.App.Clien.Longitud, 0);
        }

        public static string GetArtic(string Codigo)
        {
            return GetCodigo(Codigo, Conf.App.Producto.Longitud, 0);
        }

        public static string GetFamilia(string Codigo)
        {
            return GetCodigo(Codigo, Conf.App.Familia.Longitud, 0);
        }

        public static string GetTarifa(string Codigo)
        {
            return GetCodigo(Codigo, Conf.App.Tarifa.Longitud, 0);
        }

        public static string GetDocClave(object clave)
        {
            return Data.ToString(clave).Substring(0, 2);
        }

        public static string GetTpago(string Codigo)
        {
            return GetCodigo(Codigo, Conf.App.TPago.Longitud, 0);
        }

        public static string GetTipClien(object clave, string serie = "")
        {
            string codigo = clave.ToString();
            int len = 6 - serie.Length;

            return serie + NormCode(codigo, len);
        }

        public static string GetAlmacen(string Codigo)
        {
            int len = Conf.App.Almacen.Longitud;

            if (len < 3)
                len = 3;

            return NormCode(Codigo, len);
        }

        public static string GetAgente(string Codigo)
        {
            return NormCode(Codigo, Conf.App.Agente.Longitud);
        }

        public static string GetCaja(string Codigo)
        {
            return NormCode(Codigo, Conf.App.Tesor.Longitud);
        }

        public static string GetPais(string Codigo)
        {
            if (Str.Empty(Codigo))
                Codigo = "ES";

            return NormCode(Codigo, 2);
        }

        public static string GetInter(string Codigo)
        {
            return NormCode(Codigo, 4);
        }

        public static string GetTerm(string Codigo)
        {
            return NormCode(Codigo, 3);
        }

        public static string GetIdBase(string ident)
        {
            if (ident != null)
            {
                if (ident.Length > 8)
                    ident = ident.Substring(0, 8);
            }

            return ident;
        }


        public static string GetSerie(string Codigo)
        {
            int LonSer = Conf.App.Docum.LonSerie;

            if (LonSer == 0)
                LonSer = 2;

            if (Codigo.Length > LonSer)
                Codigo = Codigo.Substring(0, LonSer);
            else
            {
                if (Str.Empty(Codigo))
                    return "".PadRight(LonSer, ' ');

                Codigo = Codigo.PadRight(LonSer, '0');
            }

            return Codigo;
        }

        #endregion 
    }
}
