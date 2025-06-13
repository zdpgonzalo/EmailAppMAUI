namespace MailAppMAUI.General
{
    /// <summary>
    /// Clase estatica que permite normalizar / formatear diferentes tipos de atributos.
    /// </summary>
    public static class Normalizar
    {
        /// <summary>
        /// Metodo que normaliza un id para evitar errores al buscar por ids.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>El <paramref name="id"/> normalizado en forma de cadena de texto.</returns>
        public static string Ids(uint id)
        {
            int numCharacters = uint.MaxValue.ToString().Length;
            return id.ToString("D" + numCharacters);
        }

        ///// <summary>
        ///// Metodo que normaliza los precios para que tengan el numero de cifras decimales y enteras especificados en la configuracion.
        ///// </summary>
        ///// <param name="precio"></param>
        ///// <returns>El <paramref name="precio"/> normalizado degun las opciones de configuracion.</returns>
        //public static double Precios(double precio)
        //{
        //    return double.Round(precio, ConfigCore.Instance.Precios.DecimalesPrecios, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza los precios para que tengan el numero de cifras decimales y enteras especificadas en la configuracion, solo que modifica el valor original.
        ///// </summary>
        ///// <param name="precio"></param>
        //public static void PreciosChange(ref double precio)
        //{
        //    precio = double.Round(precio, ConfigCore.Instance.Precios.DecimalesPrecios, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza las cantidades para que tengan el numero de cifras decimales y enteras especificado en la configuracion.
        ///// </summary>
        ///// <param name="cantidad"></param>
        ///// <returns>La <paramref name="cantidad"/> normnalizada segun las opciones de configuracion.</returns>
        //public static double Cantidades(double cantidad)
        //{
        //    return double.Round(cantidad, ConfigCore.Instance.Precios.DecimalesCantidades, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza las cantidades para que tengan el numero de cifras decimales y enteras especificado en la configuracion, salvo que este modifica la cantidad original.
        ///// </summary>
        ///// <param name="cantidad"></param>
        //public static void CantidadesChange(ref double cantidad)
        //{
        //    cantidad = double.Round(cantidad, ConfigCore.Instance.Precios.DecimalesCantidades, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza los porcentajes, primero convirtiendolos a rango 0-1, y luego segurandose que tengan las cifras decimales y enteras especificadas en la configuracion.
        ///// </summary>
        ///// <param name="porcentaje"></param>
        ///// <returns>El <paramref name="porcentaje"/> nornmalizado segun las opciones de configuracion y en un rnago de 0-1.</returns>
        //public static double Porcentajes(double porcentaje)
        //{
        //    if (porcentaje >= 1)
        //    {
        //        porcentaje /= 100;
        //    }
        //    return double.Round(porcentaje, ConfigCore.Instance.Precios.DecimalesDescuentos + 2, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza los porcentajes, primero convirtiendolos a rango 0-1, y luego segurandose que tengan las cifras decimales y enteras especificadas en la configuracion, salvo que este modifica el porcentaje original.
        ///// </summary>
        ///// <param name="porcentaje"></param>
        //public static void PorcentajesChange(ref double porcentaje)
        //{
        //    if (porcentaje >= 1)
        //    {
        //        porcentaje /= 100;
        //    }
        //    porcentaje = double.Round(porcentaje, ConfigCore.Instance.Precios.DecimalesDescuentos + 2, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza los totales para que tenagn el numero de cifras decimales y enteras especificadas en la configuracion.
        ///// </summary>
        ///// <param name="total"></param>
        ///// <returns>El <paramref name="total"/> normalizado segun las opciones de configuracion.</returns>
        //public static double Totales(double total)
        //{
        //    return double.Round(total, ConfigCore.Instance.Precios.DecimalesTotales, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que normaliza los totales para que tenagn el numero de cifras decimales y enteras especificadas en la configuracion, salvo que este modifica el total original.
        ///// </summary>
        ///// <param name="total"></param>
        //public static void TotalesChange(ref double total)
        //{
        //    total = double.Round(total, ConfigCore.Instance.Precios.DecimalesTotales, MidpointRounding.AwayFromZero);
        //}

        ///// <summary>
        ///// Metodo que nomaliza el codigo de los clientes para que complan con las opciones de configuracion.
        ///// </summary>
        ///// <param name="codigo"></param>
        ///// <returns>El <paramref name="codigo"/> normalizado en funcion de lo dictado por la configuracion.</returns>
        //public static string CodClientes(string codigo)
        //{
        //    switch (ConfigCore.Instance.Codigos.Formatear)
        //    {
        //        case TipoFormateo.ZerosIzq:
        //            if (codigo != string.Empty && int.TryParse(codigo[0].ToString(), out _))
        //            {
        //                int numZeros = ConfigCore.Instance.Codigos.LongCodigoClientes - codigo.Length;
        //                if (numZeros > 0)
        //                {
        //                    for (int i = 0; i < numZeros; i++)
        //                    {
        //                        codigo = 0.ToString() + codigo;
        //                    }
        //                    return codigo;
        //                }
        //            }
        //            return codigo;
        //        case TipoFormateo.ZerosDer:
        //            if (codigo != string.Empty && int.TryParse(codigo[0].ToString(), out _))
        //            {
        //                int numZeros = ConfigCore.Instance.Codigos.LongCodigoClientes - codigo.Length;
        //                if (numZeros > 0)
        //                {
        //                    for (int i = 0; i < numZeros; i++)
        //                    {
        //                        codigo += 0.ToString();
        //                    }
        //                    return codigo;
        //                }
        //            }
        //            return codigo;

        //        default:
        //            return codigo;
        //    }
        //}

        ///// <summary>
        ///// Metodo que nomaliza el codigo de los productos para que complan con las opciones de configuracion.
        ///// </summary>
        ///// <param name="codigo"></param>
        ///// <returns>El <paramref name="codigo"/> normalizado en funcion de lo dictado por la configuracion.</returns>

        //public static string CodProductos(string codigo)
        //{
        //    switch (ConfigCore.Instance.Codigos.Formatear)
        //    {
        //        case TipoFormateo.ZerosIzq:
        //            if (codigo != string.Empty && int.TryParse(codigo[0].ToString(), out _))
        //            {
        //                int numZeros = ConfigCore.Instance.Codigos.LongCodigoProductos - codigo.Length;
        //                if (numZeros > 0)
        //                {
        //                    for (int i = 0; i < numZeros; i++)
        //                    {
        //                        codigo = 0.ToString() + codigo;
        //                    }
        //                    return codigo;
        //                }
        //            }
        //            return codigo;
        //        case TipoFormateo.ZerosDer:
        //            if (int.TryParse(codigo[0].ToString(), out _))
        //            {
        //                int numZeros = ConfigCore.Instance.Codigos.LongCodigoProductos - codigo.Length;
        //                if (numZeros > 0)
        //                {
        //                    for (int i = 0; i < numZeros; i++)
        //                    {
        //                        codigo += 0.ToString();
        //                    }
        //                    return codigo;
        //                }
        //            }
        //            return codigo;

        //        default:
        //            return codigo;
        //    }
        //}

        ///// <summary>
        ///// Metodo que genera el codigo de un documento.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns>Un codigo para un documento en base a su id y unas opciones de configuracion.</returns>
        //public static string CodDocumentos(uint id)
        //{
        //    return ConfigCore.Instance.Codigos.SerieDefault + id.ToString(ConfigCore.Instance.Codigos.DocumentoCodFormat);
        //}

        //public static double Peso(double peso)
        //{
        //    double factorConversion;
        //    switch (ConfigCore.Instance.General.UnidadesPeso)
        //    {
        //        case UnidadesPeso.Miligramos:
        //            factorConversion = 1000000d;
        //            break;

        //        case UnidadesPeso.Gramos:
        //            factorConversion = 1000d;
        //            break;

        //        case UnidadesPeso.Toneladas:
        //            factorConversion = 0.001d;
        //            break;

        //        default:
        //            factorConversion = 1d;
        //            break;
        //    }
        //    return peso * factorConversion;
        //}

        //public static double Volumen(double volumen)
        //{
        //    double factorConversion;
        //    switch (ConfigCore.Instance.General.UnidadesVolumen)
        //    {
        //        case UnidadesVolumen.Cm3:
        //            factorConversion = 1000000d;
        //            break;

        //        case UnidadesVolumen.CentiLitros:
        //            factorConversion = 100000d;
        //            break;

        //        case UnidadesVolumen.Litros:
        //            factorConversion = 1000d;
        //            break;

        //        default:
        //            factorConversion = 1d;
        //            break; ;
        //    }
        //    return volumen * factorConversion;
        //}

        //public static string PesoWithUnidades(double peso)
        //{
        //    string unidad;
        //    switch (ConfigCore.Instance.General.UnidadesPeso)
        //    {
        //        case UnidadesPeso.Miligramos:
        //            unidad = "mg";
        //            break;

        //        case UnidadesPeso.Gramos:
        //            unidad = "g";
        //            break;

        //        case UnidadesPeso.Toneladas:
        //            unidad = "t";
        //            break;

        //        default:
        //            unidad = "kg";
        //            break;
        //    }
        //    return $"{peso} {unidad}";
        //}

        //public static string VolumenWithUnidades(double volumen)
        //{
        //    string unidad;
        //    switch (ConfigCore.Instance.General.UnidadesVolumen)
        //    {
        //        case UnidadesVolumen.Cm3:
        //            unidad = "cm3";
        //            break;

        //        case UnidadesVolumen.CentiLitros:
        //            unidad = "cl";
        //            break;

        //        case UnidadesVolumen.Litros:
        //            unidad = "L";
        //            break;

        //        default:
        //            unidad = "m3";
        //            break;
        //    }
        //    return $"{volumen} {unidad}";
        //}

        public static string Terminal(string terminal)
        {
            if (string.IsNullOrEmpty(terminal))
            {
                return "000";
            }

            for (int i = terminal.Length; i < 3; i++)
            {
                terminal = $"0{terminal}";
            }

            if (terminal.Length > 3)
            {
                return terminal[^3..];
            }

            return terminal;
        }

        public static string Terminal(uint terminalId)
        {
            return Terminal(terminalId.ToString());
        }

        public static void TerminalChange(ref string terminal)
        {
            if (string.IsNullOrEmpty(terminal))
            {
                terminal = "000";
                return;
            }

            for (int i = terminal.Length; i < 3; i++)
            {
                terminal = $"0{terminal}";
            }

            if (terminal.Length > 3)
            {
                terminal = terminal[^3..];
                return;
            }
        }

        public static string Codigo(string codigo, int digits)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                return new('0', digits);
            }

            if (codigo.Length > digits)
            {
                return codigo[^digits..];
            }

            for (int i = codigo.Length; i < digits; i++)
            {
                codigo = $"0{codigo}";
            }

            return codigo;
        }

        public static int NivelCambio(NivelCambio nivelCambio)
        {
            switch (nivelCambio)
            {
                case General.NivelCambio.Alta:
                    return 10;

                case General.NivelCambio.Modificacion:
                    return 21;

                case General.NivelCambio.Baja:
                    return 30;

                case General.NivelCambio.Enviado:
                    return 23;

                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Clase estatica que permite desnormalizar los atributos y propiedades que estén sujetos a normalizacion.
    /// </summary>
    public static class Desnormalizar
    {
        /// <summary>
        /// Metodo que desnormaliza un id para convertirlo a numero.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static uint Ids(string id)
        {
            if (uint.TryParse(id, out uint num))
            {
                return num;
            }
            return 0u;
        }

        ///// <summary>
        ///// Metodo que desnormaliza el codigo de los clientes en base a las opciones de configuracion.
        ///// </summary>
        ///// <param name="codigo"></param>
        ///// <returns>El <paramref name="codigo"/> desnormalizado.</returns>
        //public static string CodClientes(string codigo)
        //{
        //    if (ConfigCore.Instance.Codigos.Formatear)
        //    {
        //        string[] partesCod = codigo.Split('_');
        //        if (!int.TryParse(partesCod[1], out int i))
        //        {
        //            return codigo;
        //        }
        //        return partesCod[0] + "_" + i.ToString(ConfigCore.Instance.Codigos.ProductoCodFormat);
        //    }
        //    return codigo;
        //}

        ///// <summary>
        ///// Metodo que desnormaliza el codigo de los productos en base a las opciones de configuracion.
        ///// </summary>
        ///// <param name="codigo"></param>
        ///// <returns>El <paramref name="codigo"/> desnormalizado.</returns>

        //public static string CodProductos(string codigo)
        //{
        //    if (ConfigCore.Instance.Codigos.Formatear)
        //    {
        //        string[] partesCod = codigo.Split('_');
        //        if (!int.TryParse(partesCod[1], out int i))
        //        {
        //            return codigo;
        //        }
        //        return partesCod[0] + "_" + i.ToString();
        //    }
        //    return codigo;
        //}

        //public static double Peso(double peso)
        //{
        //    var factorConversion = ConfigCore.Instance.General.UnidadesPeso switch
        //    {
        //        UnidadesPeso.Miligramos => 0.000001d,
        //        UnidadesPeso.Gramos => 0.001d,
        //        UnidadesPeso.Toneladas => 1000d,
        //        _ => 1d,
        //    };
        //    return peso * factorConversion;
        //}

        //public static double Volumen(double volumen)
        //{
        //    double factorConversion = ConfigCore.Instance.General.UnidadesVolumen switch
        //    {
        //        UnidadesVolumen.Cm3 => 0.000001d,
        //        UnidadesVolumen.CentiLitros => 0.00001d,
        //        UnidadesVolumen.Litros => 0.001d,
        //        _ => 1d,
        //    };
        //    return volumen * factorConversion;
        //}

        public static double MedidaWithUnidades(string medida)
        {
            if (double.TryParse(medida, out double pesoD))
            {
                return pesoD;
            }
            string[] pesoDiv = medida.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (double.TryParse(pesoDiv[^1], out pesoD))
            {
                return pesoD;
            }
            return 0d;
        }

        public static uint Terminal(string terminal)
        {
            if (string.IsNullOrEmpty(terminal))
            {
                return default;
            }

            if (!uint.TryParse(terminal, out uint id))
            {
                return default;
            }

            return id;
        }

        public static NivelCambio NivelCambio(int nivelCambio)
        {
            switch (nivelCambio)
            {
                case 10:
                    return General.NivelCambio.Alta;

                case 21:
                    return General.NivelCambio.Modificacion;

                case 23:
                    return General.NivelCambio.Enviado;

                case 30:
                    return General.NivelCambio.Baja;

                default:
                    return General.NivelCambio.None;
            }
        }
    }
}