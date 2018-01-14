using CustomVisionService.Model;
using CustomVisionService.Services;
using System.Collections.ObjectModel;
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
            set { _isBusy = value; OnPropertyChanged();
                TirarCommand.ChangeCanExecute();
                AbrirCommand.ChangeCanExecute();
                StreamCommand.ChangeCanExecute();
                ServicoCommand.ChangeCanExecute();
            }
        }

        Stream _imagemStream;
        private readonly string key = ""; //Chave do seriviço

        //A url muda para cada tipo de consulta, por URL ou por arquivo de foto
        private readonly string url = ""; // Sua URL do serviço
        private readonly string urlStream = ""; 

        public Command ServicoCommand { get; }
        public Command TirarCommand { get; }
        public Command AbrirCommand { get; }
        public Command StreamCommand { get; }
        CVService Servico;
        CVSModel Model;
        public ObservableCollection<Prediction> MyPrediction { get; set; }

        public PrincipalViewModel()
        {
            ServicoCommand = new Command(ExecuteServicoCommand,()=>!IsBusy);
            StreamCommand = new Command(async () => await ExecuteStreamCommand(), ()=>!IsBusy);
            TirarCommand = new Command(async () => await ExecuteTirarCommand(), () => !IsBusy);
            AbrirCommand = new Command(async () => await ExecuteAbrirCommand(), () => !IsBusy);
        
            Model = new CVSModel();
            MyPrediction = new ObservableCollection<Prediction>();
        }

        async Task ExecuteStreamCommand()
        {

            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    Servico = new CVService(key, urlStream);
                    if (_imagemStream == null)
                        await DisplayAlert("Erro", "Nenhuma imagem selecionada", "Ok");
                    Model = await Servico.CVSStreamAsync(_imagemStream);
                    ExibirLista(Model);

                }
                catch (System.Exception ex)
                {

                    await DisplayAlert("Erro!", $"Erro:{ex.Message}", "Ok");
                }

                finally
                {
                    IsBusy = false;
                }
            }

            return;
        }

        async Task ExecuteAbrirCommand()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await CrossMedia.Current.Initialize();
                    var arquivoFoto = await CrossMedia.Current.PickPhotoAsync();

                    _imagemStream = arquivoFoto?.GetStream();
                    Url = arquivoFoto?.Path;

                }
                catch (System.Exception ex)
                {

                    await DisplayAlert("Erro!", $"Erro:{ex.Message}", "Ok");
                }

                finally
                {
                    IsBusy = false;
                }
            }

            return;
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


            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    Servico = new CVService(key, url);
                    Model = await Servico.CVSUrlAsync(Url);
                    ExibirLista(Model);

                }
                catch (System.Exception ex)
                {

                    await DisplayAlert("Erro!", $"Erro:{ex.Message}", "Ok");
                }

                finally
                {
                    IsBusy = false;
                }
            }

            return;
          
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

