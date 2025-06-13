//using EmailApp.Gestion;
//using EmailApp.Modelos.Core;
//using EmailApp.UseCases;
//using EmailAppGonzalo.Dominio.Config;
//using iTextSharp.text.pdf;
//using Newtonsoft.Json;
//using System.Reflection.Metadata;
//using System.Text;
//using iTextSharp.text;
//using Document = iTextSharp.text.Document;
//using iTextSharp.text.pdf.parser;
//using System.Drawing;
//using System.Drawing.Imaging;
//using PdfiumViewer;
//using System.IO;
//using System.Runtime.Intrinsics.Arm;

//namespace EmailAppGonzalo.Aplicacion.UseCases
//{
//    public class DesglosarFacturasIA
//    {
//        private static readonly string apiKey = "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"; // Tu clave de API
//        private static readonly string apiUrl = "https://api.openai.com/v1/chat/completions"; // URL de la API


//        //RUTA DE GUARDAR ARCHIVOS
//        static Configuration Conf { get; set; }

//        public DesglosarFacturasIA()
//        {
//            if ((Conf = Configuration.Config) == null)
//            {
//                Conf = new Configuration();
//            }
//        }

//        /// <summary>
//        /// Desglosa la información del PDF
//        /// </summary>
//        /// <param name="adjuntoPDF">PDF a desglosar</param>
//        /// <param name="correoConPDF">Correo al que corresponde el PDF</param>
//        /// <returns>El desglose del PDF en formato JSON</returns>
//        public async Task<Respuesta> DesglosarPDF(Adjunto adjuntoPDF, Correo correoConPDF)
//        {
//            string facturaDesglose = await ConvertPDFtoText(adjuntoPDF);

//            using (var client = new HttpClient())
//            {
//                // Configura la clave de la API en los encabezados
//                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

//                var prompt = $"Comprueba si se trata de una factura o no lo siguiente: {facturaDesglose}. En caso afirmativo haz lo siguiente teniendo en cuenta que la información del PDF se ha extraído por cordenadas X,Y: " +
//                    $"Extrae el número de factura, fecha de emisión, Emisor, destinatario, datos del envío, numero de pedido o presupuesto,  base imponible, IVA total, total factura y líneas de detalle con cantidad, código, descripción, precio y total del siguiente documento." +
//                    $"\r\n\r\nSi no hay una etiqueta al lado de datos clave como numero de factura o fecha, comprueba si las etiquetas están en la línea anterior." +
//                    $"\r\n\r\nVerifica en los datos extraídos que el total de la factura es igual a la base imponible más el IVA y que la suma de bases y totales de cada linea de detalle coincide con la base e iva total" +
//                    $"\r\n\r\nAsegúrate de que los valores sean correctos y devuélvelos en JSON. ";

//                var context = "Eres un asistente útil para desglosar las facturas";

//                // Crea el cuerpo de la petición (JSON)
//                var requestBody = new
//                {
//                    model = "gpt-4-turbo",
//                    messages = new[]
//                    {
//                         new { role = "system", content = context },
//                         new { role = "user", content = prompt }
//                     },
//                    max_tokens = 600,
//                    temperature = 0.7
//                };

//                // Convierte el objeto a JSON
//                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

//                // Realiza la solicitud POST
//                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

//                var response = await client.PostAsync(apiUrl, content); //QUITAR CONF AWAIT SI NO FUFA

//                try
//                {
//                    // Si la solicitud es exitosa, procesa la respuesta
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        throw new Exception("Solicitud incorrecta. Respuesta no proporcionada");
//                    }
//                    var responseBody = await response.Content.ReadAsStringAsync();

//                    // Extrae la respuesta del modelo desde el JSON de respuesta y la deserializa en mi clase OpenAIResponse (por ahora está escrita abajo)
//                    var jsonResponse = JsonConvert.DeserializeObject<OpenAiResponse>(responseBody);
//                    string respuestaTexto = jsonResponse?.choices?.FirstOrDefault()?.message?.content?.Trim() ?? "Error al generar respuesta";

//                    Respuesta miRespuesta = Respuesta.CreateRespuesta(correoConPDF, correoConPDF.Remitente, correoConPDF.Destinatarios, "asunto", respuestaTexto, respuestaTexto, DateTime.Now);

//                    // Devolvemos la respuesta
//                    return miRespuesta;
//                }
//                catch (Exception ex)
//                {
//                    WebLog.LogError(ex);
//                    return null;
//                }
//            }
//        }

//        private async Task<string> ConvertPDFtoText(Adjunto adjuntoPDF)
//        {
//            /*string pdfText = "";

//            StringBuilder sb = new StringBuilder();

//             using (PdfReader reader = new PdfReader(adjuntoPDF.Ruta))
//             {
//                 for (int page = 1; page <= reader.NumberOfPages; page++)
//                 {
//                     ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

//                     string text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

//                     text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));

//                     sb.Append(text);
//                 }
//             }




//            //Forma más clara
//            using (PdfReader reader = new PdfReader((adjuntoPDF.Ruta)))
//            {
//                string texto = "";
//                for (int i = 1; i <= reader.NumberOfPages; i++)
//                {
//                    texto += PdfTextExtractor.GetTextFromPage(reader, i) + "\n";
//                }
//                texto = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(texto)));

//                return texto;
//            }*/

//            string str = "2025-01-03\r\n19100\r\n29 100204 29\r\n9 123947 2 ES\r\n000 29\r\n608 221 402 JOSE LUIS MATA CARRASCO\r\nA JOSE LUIS MATA CARRASCOSA\r\nO C/ GENERAL BRAVO 32\r\nD LAS PALMAS G ES\r\nA 35002 DE 35002\r\nI\r\nV\r\nN\r\nE\r\nFACTURA INFOSER SISTEMAS SA\r\nCL SANTA VIRGILIA, 3 BAJOS B\r\nA\r\nO MADRID\r\nD I 28033 MADRID\r\nREMITE: D\r\nN\r\nE\r\nINGRAM MICRO, S.L.U. V\r\nCALLE CANTABRIA,2 PLTA 3 PTA B1\r\n28108 ALCOBENDAS MADRID SPAIN N.I.F.\r\nA78648144\r\nC.I.F.\r\n- -\r\nNIF ESB78076395\r\nFECHA DE No. DE\r\n. PEDIDO 27/12/24 5160411\r\nPEDIDO\r\n5\r\n9\r\n3\r\n6\r\n7\r\n0 NUMERO FACTURA FORMA DE NUMERO CLIENTE FECHA DE\r\nTRANSPORTE PAGINA\r\n8 NUMERO PEDIDO\r\nDE\r\n7 PAGO FACTURA\r\nB\r\nF I\r\nSPECIAL S DIAS\r\nC 291002049 30 29-123947-000 19100 03-01-2025 1\r\n,\r\n6\r\n/ A NO ART. PED. ENTR. DESCRIPCION PRECIO PARCIAL\r\nI IVA %\r\n,\r\n5\r\n5 1\r\n6\r\n9\r\n5\r\n6 1\r\nM\r\nH CL06033 1 1 TMP414-53 CI520U KIT DIGITAL SYS 665,00 665,00 21.0%\r\n,\r\n8\r\nS\r\n, 16GB 512SSD 14 IN W11P\r\n4\r\n1\r\nF VENDOR PART: NX.B73EB.002\r\n,\r\n9\r\n5 INTRASTAT 8471 30 00 WEIGHT 002.44\r\n8 : :\r\n6\r\n3\r\nT SERIAL NBR: NXB73EB0024301BA967600\r\n,\r\nd i\r\nr CL14371 1 1 SERVICIOS DE LABORATORIO BTO 4,50 4,50 21.0%\r\nd\r\na\r\nM\r\ne IN WH ACER\r\nd l -\r\ni t\r\nVENDOR KIT.DIG-SERV.INWH-A\r\nPART:\r\nn\r\na c\r\nr\r\nINTRASTAT 0000 00 00\r\ne :\r\nM\r\no CL14372 1 1 SOLU. CIBERSEGURIDAD/SOPORTE BTO 59,50 59,50 21.0%\r\nr\r\nt\r\ns\r\ni\r\ng FORMACIN EVIDENCIAS ACER\r\ne +\r\nR l -\r\ne VENDOR PART: KIT.DIG-SERV.OUTWH-\r\nn\r\ne\r\nt a INTRASTAT : 0000 00 00\r\ni\r\nr\r\nc\r\ns CL14357 1 1 MENSAJERA: ENTREGA CON POD STD 4,30 4,30 21.0%\r\nI n\r\n-\r\n8 PENNSULA ACER\r\n0\r\n1 -\r\n8\r\n2 VENDOR PART: KIT.DIG-TRNS1-AC\r\ns\r\na\r\nd INTRASTAT 0000 00 00\r\nn :\r\ne\r\nb 1\r\no\r\nc\r\nl\r\nA 1 CANON LPI TABL,MOVIL,LAPTOP 5,33 5,33 21.0%\r\n1\r\nB\r\nt a CL06033 1 5,3300\r\nEUR\r\nn x\r\nl a\r\nP 1 CARGO POR ENVIO DE MATERIAL\r\nª\r\n3\r\n,\r\n2\r\ni a\r\nr\r\nb\r\nt a\r\nn\r\na\r\nC\r\n/\r\nC\r\n.\r\na r\r\nu\r\nm\r\nA\r\n.\r\nd\r\nE\r\n, .\r\nU . El cliente derecho obtener el reembolso del correspondiente al cumpla los el artículo del\r\ntiene a importe canon LPI siempre que con requisitos previstos en 25.8\r\nL\r\n.\r\nS texto refundido recogido en el RDL 12/2017 de 3 de julio\r\n,\r\no r\r\nc\r\ni\r\nM\r\nVENCIM.: 02/02/25 INGRAM MICRO S.L. es productor de pilas y acumuladores con Nº RII-PYA 81, adherido a ECOPILAS y productor de AEES con Nº RAEE 867, adherido a\r\nm\r\nECOASIMELEC. N.º de registro de ENV/2023/000003896\r\na r envases:\r\ng\r\nI n SUBTOTAL DTO. FIN. BASE IMPONIBLE %IVA CUOTA IVA TOTAL FACTURA\r\n738,63 0,00 738,63 21.0 155,12\r\n893,75 EUR\r\nLA ENTREGA DE LA MERCANCIA SUPONDRA LA TOTAL ACEPTACION DE LAS SIGUIENTES CLAUSULAS POR PARTE DEL CLIENTE:\r\n1. RESERVA DE DOMINIO: El material entregado continuará en propiedad de INGRAM MICRO. Hasta el total pago de su importe constituyéndose\r\n-\r\nmientras tanto en depósito en el domicilio del cliente. Este queda nombrado depositario con los efectos previstos en el art. 306 del Código de Comercio.\r\n2. CONDICIONES DE DISTRIBUCIÓN El cliente da por enterado y acepta los Términos y Condiciones Generales de Venta disponibles para\r\nse su\r\n-\r\nconsulta en nuestra página de internet: https://es-new.ingrammicro.com/site/cms?page=Services-and-Support/Condiciones-Comerciales-y-Legales\r\n";

//            return str;
//        }
//    }
//}
