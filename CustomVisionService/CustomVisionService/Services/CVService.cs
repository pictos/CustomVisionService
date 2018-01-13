using CustomVisionService.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CustomVisionService.Services
{
    public class CVService
    {
        HttpClient _client;
        private readonly string _key;
        private readonly string _url;
         

        public CVService(string key, string url)
        {
            _key = key;
            _url = url;
        }

        public async Task<CVSModel> CVSUrlAsync(string urlImagem)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Prediction-Key", _key);

            HttpResponseMessage resposta;

            var stringContent = new StringContent(@"{""Url"":""" + urlImagem + @"""}");
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            resposta = await _client.PostAsync(_url, stringContent);
            var json = await resposta.Content.ReadAsStringAsync();
            if (resposta.IsSuccessStatusCode)
            {
                var resultado = JsonConvert.DeserializeObject<CVSModel>(json);

                //  await App.Current.MainPage.DisplayAlert("Deu certo", "resultado", "ok");
                return resultado;
            }
            else
            {
                throw new Exception(json);
            }
        }

        public async Task<CVSModel> CVSStreamAsync(Stream stream)
        {
            
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Prediction-Key", _key);

            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            try
            {
                HttpResponseMessage resposta = await _client.PostAsync(_url,streamContent);
                var json = await resposta.Content.ReadAsStringAsync();

                if(resposta.IsSuccessStatusCode)
                {
                    var resultado = JsonConvert.DeserializeObject<CVSModel>(json);
                    return resultado;
                }
                throw new Exception(json);
            }
            catch (Exception ex)
            {

                throw ex;
            }

           
        }
    }
}
