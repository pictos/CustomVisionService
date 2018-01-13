using CustomVisionService.Model;
using CustomVisionService.Services;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;
using Plugin.Media;
using System.IO;
using Plugin.Media.Abstractions;

namespace CustomVisionService.ViewModel
{
    public class PrincipalViewModel : BaseViewModel
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; OnPropertyChanged(); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); TirarCommand.ChangeCanExecute();AbrirCommand.ChangeCanExecute(); }
        }

        Stream _imagemStream;
        private readonly string key = "ae338ccf095f437bb7b0c1eac7da0354";
        private readonly string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/Prediction/93202177-b70b-4ee0-9603-069bf8f6f4b9/url";
        private readonly string urlStream = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/Prediction/93202177-b70b-4ee0-9603-069bf8f6f4b9/image";

        public Command ServicoCommand { get; }
        public Command TirarCommand { get; }
        public Command AbrirCommand { get; }
        public Command StreamCommand { get; }
        CVService Servico;
        CVSModel Model;
        public ObservableCollection<Prediction> MyPrediction { get; set; }

        public PrincipalViewModel()
        {
            ServicoCommand = new Command(ExecuteServicoCommand);
            StreamCommand = new Command(ExecuteStreamCommand);
            TirarCommand = new Command(async () => await ExecuteTirarCommand(), () => !IsBusy);
            AbrirCommand = new Command(async () => await ExecuteAbrirCommand(), () => !IsBusy);
        
            Model = new CVSModel();
            MyPrediction = new ObservableCollection<Prediction>();
        }

        async void ExecuteStreamCommand()
        {
            Servico = new CVService(key, urlStream);
            Model = await Servico.CVSStreamAsync(_imagemStream);
            ExibirLista(Model);
        }

        async Task ExecuteAbrirCommand()
        {
            await CrossMedia.Current.Initialize();
            var arquivoFoto = await CrossMedia.Current.PickPhotoAsync();

            _imagemStream = arquivoFoto?.GetStream();
            Url = arquivoFoto?.Path;
        }

        async Task ExecuteTirarCommand()
        {
            if (!IsBusy)
            {
                Exception Erro = null;

                try
                {
                    IsBusy = true;
                    await CrossMedia.Current.Initialize();
                    var arquivoFoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());

                    _imagemStream = arquivoFoto?.GetStream();
                    Url = arquivoFoto?.Path;

                   
                }
                catch (Exception ex)
                {

                    Erro = ex;
                    await DisplayAlert("Erro", Erro.Message, "Ok");
                }
                finally
                {
                    IsBusy = false;
                }

            }
            return;
        }

        async void ExecuteServicoCommand()
        {
            Servico = new CVService(key, url);
            Model = await Servico.CVSUrlAsync(Url);

            ExibirLista(Model);
        }

        private void ExibirLista(CVSModel Model)
        {
            MyPrediction.Clear();

            foreach (var item in Model.Predictions)
            {
                MyPrediction.Add(new Prediction
                {
                    Tag = item.Tag,
                    TagId = item.TagId,
                    Probability = item.Probability
                });
            }
        }
    }
}
