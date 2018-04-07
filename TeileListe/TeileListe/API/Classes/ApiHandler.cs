using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using TeileListe.API.PostClasses;
using TeileListe.API.ResponseClasses;

namespace TeileListe.API.Classes
{
    public class ApiHandler
    {
        private readonly JsonParser _parser;
        private readonly JavaScriptSerializer _deSerializer;

        public ApiHandler()
        {
            _parser = new JsonParser();
            _deSerializer = new JavaScriptSerializer();
        }

        private string GetResponse(string url, string apiToken)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers = new WebHeaderCollection { "api-token: " + apiToken };
            try
            {
                var result = string.Empty;
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                        reader.Close();
                        responseStream.Close();
                    }
                    response.Close();
                }
                return result;
            }
            catch (WebException ex)
            {
                var errorText = ex.Message;
                var errorResponse = ex.Response;
                if (errorResponse != null)
                {
                    using (var responseStream = errorResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                            var response = reader.ReadToEnd();
                            var errorDto = (ErrorResponseDt)_parser.ConvertJson(typeof(ErrorResponseDt), response);
                            if (errorDto != null)
                            {
                                errorText += Environment.NewLine;
                                if (errorDto.Messages != null && errorDto.Messages.Count > 0)
                                {
                                    errorText = errorDto.Messages.Aggregate(errorText,
                                        (current, message) =>
                                            current + (Environment.NewLine + EntferneMaskierteZeichen(message)));
                                }
                                if (errorDto.Data != null && errorDto.Data.Count > 0)
                                {
                                    errorText = errorDto.Data.Aggregate(errorText,
                                        (current, message) =>
                                            current + (Environment.NewLine + EntferneMaskierteZeichen((message))));
                                }
                            }
                            reader.Close();
                            responseStream.Close();
                        }
                        errorResponse.Close();
                    }
                }
                throw new Exception(errorText);
            }
        }

        internal ResponseKategorieBaseDto GetKategorienListe(string datenbank, string apiToken)
        {
            var kategorien = GetResponse("https://gewichte." + datenbank + "/api/v1/categories/tree.json", 
                                            apiToken);
            return (ResponseKategorieBaseDto)_parser.ConvertJson(typeof(ResponseKategorieBaseDto), 
                                                                    kategorien);
        }

        internal ResponseHerstellerBaseDto GetHerstellerListe(string datenbank, string apiToken)
        {
            var hersteller = GetResponse("https://gewichte." + datenbank + "/api/v1/manufacturers/list.json",
                                            apiToken);
            return _deSerializer.Deserialize<ResponseHerstellerBaseDto>(hersteller);
        }

        internal ResponseProduktListeDto SucheArtikel(string datenbank, 
                                                        string apiToken, 
                                                        string id, 
                                                        bool isHerstellerSuche)
        {
            var url = "https://gewichte.";
            url += datenbank;
            url += "/api/v1/products/";
            url += isHerstellerSuche ? "manufacturer/" : "category/";
            url += id;
            url += ".json";

            return (ResponseProduktListeDto)_parser.ConvertJson(typeof(ResponseProduktListeDto), 
                                                                GetResponse(url, apiToken));
        }

        internal ResponseProduktEinzelnDto SucheEinzelnenArtikel(string datenbank,
            string apiToken,
            string produktId)
        {
            var url = "https://gewichte.";
            url += datenbank;
            url += "/api/v1/products/get/";
            url += produktId;
            url += ".json";

            var produkt = GetResponse(url, apiToken);

            return (ResponseProduktEinzelnDto)_parser.ConvertJson(typeof(ResponseProduktEinzelnDto), produkt);
        }

        internal ResponseMessungDto SendMessung(string datenbank,
                                                string apiToken,
                                                string produktId,
                                                decimal gewicht, 
                                                string imageBase64)
        {
            var url = "https://gewichte.";
            url += datenbank;
            url += "/api/v1/images/add.json";

            var dto = new AddMessungDto
            {
                ProduktId = produktId,
                Gewicht = gewicht,
                ImageBase64 = imageBase64
            };

            var ser = new DataContractJsonSerializer(typeof(AddMessungDto));
            var ms = new MemoryStream();
            ser.WriteObject(ms, dto);
            byte[] byteArray = ms.ToArray();

            var response = GetPostResponse(url, apiToken, byteArray);

            return (ResponseMessungDto)_parser.ConvertJson(typeof(ResponseMessungDto), response);
        }

        internal ResponseMessungDto SendProdukt(string datenbank,
                                                string apiToken,
                                                AddProduktDto produkt)
        {
            var url = "https://gewichte.";
            url += datenbank;
            url += "/api/v1/products/add.json";

            var ser = new DataContractJsonSerializer(typeof(AddProduktDto));
            var ms = new MemoryStream();
            ser.WriteObject(ms, produkt);
            byte[] byteArray = ms.ToArray();

            var response = GetPostResponse(url, apiToken, byteArray);

            return (ResponseMessungDto)_parser.ConvertJson(typeof(ResponseMessungDto), response);
        }

        private string GetPostResponse(string url, string apiToken, byte[] byteArray)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Headers = new WebHeaderCollection { "api-token: " + apiToken };
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            try
            {
                var dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                var response = request.GetResponse();

                var result = string.Empty;

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                        reader.Close();
                        responseStream.Close();
                    }
                    response.Close();
                }
                return result;
            }
            catch (WebException ex)
            {
                var errorText = ex.Message;
                var errorResponse = ex.Response;
                if (errorResponse != null)
                {
                    using (var responseStream = errorResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                            var response = reader.ReadToEnd();
                            var errorDto = (ErrorResponseDt)_parser.ConvertJson(typeof(ErrorResponseDt), response);
                            if (errorDto != null)
                            {
                                errorText += Environment.NewLine;
                                if (errorDto.Messages != null && errorDto.Messages.Count > 0)
                                {
                                    errorText = errorDto.Messages.Aggregate(errorText,
                                        (current, message) =>
                                            current + (Environment.NewLine + EntferneMaskierteZeichen(message)));
                                }
                                if (errorDto.Data != null && errorDto.Data.Count > 0)
                                {
                                    errorText = errorDto.Data.Aggregate(errorText,
                                        (current, message) =>
                                            current + (Environment.NewLine + EntferneMaskierteZeichen(message)));
                                }
                            }
                            reader.Close();
                            responseStream.Close();
                        }
                        errorResponse.Close();
                    }
                }
                throw new Exception(errorText);
            }
        }

        private string EntferneMaskierteZeichen(string baseStr)
        {
            return baseStr.Replace("&quot;", "\"");
        }
    }
}