using UnityEngine;
using System.Text;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HTTPHandler : MonoBehaviour
{
    [System.Serializable]
    public class DatosUsuario
    {
        public int idUsuario = -1;
        public string username;
        public int seguidores;
        public int cohesion;
        public int dinero;
    }

    [System.Serializable]
    public class VideoData
    {
        public int idVideo;
        public string theory;
        public string video_url;
    }

    [System.Serializable]
    private class VideoListWrapper
    {
        public List<VideoData> videos;
    }

    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool transactionInProgress = false;
    public DatosUsuario datosUsuario;
    public List<VideoData> listaVideos = new List<VideoData>();

    private string videosJson = @"[
      {
        ""idVideo"": 1,
        ""theory"": ""La paloma es un dron espía "",
        ""video_url"": ""https://cdn.jsdelivr.net/gh/valeriacastillolopez/VideosLoquito@main/YlaPaloma.mp4""
      },
      {
        ""idVideo"": 2,
        ""theory"": ""tomato"",
        ""video_url"": ""https://cdn.jsdelivr.net/gh/valeriacastillolopez/VideosLoquito@main/LosTacos.mp4""
      },
      {
        ""idVideo"": 3,
        ""theory"": ""Los gatos, mugrosos gatos"",
        ""video_url"": ""https://cdn.jsdelivr.net/gh/valeriacastillolopez/VideosLoquito@main/LosGatosSaben.mp4""
      }
    ]";

    void Awake()
    {
        CargarVideosDesdeJson();
    }

    private void CargarVideosDesdeJson()
    {
        string wrappedJson = "{\"videos\":" + videosJson + "}";
        //Debug.Log(wrappedJson);
        VideoListWrapper wrapper = JsonUtility.FromJson<VideoListWrapper>(wrappedJson);
        listaVideos = wrapper.videos;
        //Debug.Log(listaVideos[0].idVIdeo);
    }

    void Start()
    {
        Debug.Log("1: "+datosUsuario.idUsuario);
        datosUsuario.idUsuario=15;
        AsingVideoToUser("banana");
        //UpdateUserData("Gonzalo",500,30,500);
        //UploadStats();

    }

    public void UpdateUserData(string username, int seguidores, int cohesion, int dinero)
    {
        datosUsuario.username = username;
        datosUsuario.seguidores = seguidores;
        datosUsuario.cohesion = cohesion;
        datosUsuario.dinero = dinero;
    }

    public void UploadUser()
    {
        if(transactionInProgress) return;
        StartCoroutine(CrearUsuario("http://10.1.124.62:8080/api/usuarios/acceso"));
    }

    private IEnumerator CrearUsuario(string url)
    {
        transactionInProgress = true;
        string json = JsonUtility.ToJson(datosUsuario);
        using(UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            transactionInProgress = false;

            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);
                DatosUsuario responseDatos = JsonUtility.FromJson<DatosUsuario>(request.downloadHandler.text);
                datosUsuario.idUsuario = responseDatos.idUsuario;
                //Debug.Log("2: "+datosUsuario.idUsuario);
            }
            else
            {
                Debug.LogError("Error en POST: " + request.error);
            }
        }
    }

    public void UploadStats()
    {
        if( transactionInProgress || datosUsuario.idUsuario == -1) return;
        StartCoroutine(ActualizarEstadisticas("http://10.1.124.62:8080/api/usuarios/"));
    }

    private IEnumerator ActualizarEstadisticas(string url)
    {
        transactionInProgress = true;
        string json = JsonUtility.ToJson(datosUsuario);
        using(UnityWebRequest request = new UnityWebRequest(url+datosUsuario.idUsuario+"/estadisticas", "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            transactionInProgress = false;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                //Debug.Log("2: "+datosUsuario.idUsuario);
            }
            else
            {
                Debug.LogError("Error en POST: " + request.error);
            }
        }
    }

    public void AsingVideoToUser(string theory)
    {
        if( transactionInProgress || datosUsuario.idUsuario == -1) return;
        StartCoroutine(AsignarVideoAJugador("http://10.1.124.62:8080/api/usuarios/",theory));
    }

    private IEnumerator AsignarVideoAJugador(string url, string theory)
    {
        int idVideo=GetIdVideoByTheory(theory);
        if(idVideo == -1) yield break;
        using(UnityWebRequest request = new UnityWebRequest(url+datosUsuario.idUsuario+"/videos/"+idVideo, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            transactionInProgress = false;
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                //Debug.Log("2: "+datosUsuario.idUsuario);
            }
            else
            {
                Debug.LogError("Error en POST: " + request.error);
            }
        }
    }

    public int GetIdVideoByTheory(string theorySearch)
    {
        VideoData videoEncontrado = listaVideos.Find(v => v.theory == theorySearch);

        if (videoEncontrado != null)
        {
            Debug.Log("Video encontrado encontrado Id: "+videoEncontrado.idVideo);
            return videoEncontrado.idVideo;
        }

        Debug.Log("No se encontró un video con la theory: " + theorySearch);
        return -1;
    }


   /*void Start()
    {
        StartCoroutine(HacerGet("http://10.1.124.62:8080/api/usuarios"));
    }

    IEnumerator HacerGet(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Opcional: Agregar cabeceras (headers) como tokens de autorización
            request.SetRequestHeader("Authorization", "Bearer TU_TOKEN_AQUI");

            // Enviar la solicitud y esperar respuesta
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Respuesta exitosa
                string respuestaJson = request.downloadHandler.text;
                Debug.Log("Respuesta: " + respuestaJson);
            }
            else
            {
                // Error de red o HTTP (404, 500, etc.)
                Debug.LogError("Error: " + request.error);
            }
        }
    }*/
}
