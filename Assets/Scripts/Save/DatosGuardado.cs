using System.Collections.Generic;

[System.Serializable]
public class DatosGuardado
{
    //playerStats
    public int currentMoney;
    public float hunger;
    public int followers;
    public int cohesion;
    public string username;
    public List<ConceptInfo> conceptInfos;
    //StoreTheories
    public List<ConceptInfo> shopConcepts;
    //HTTPHandler
    public HTTPHandler.DatosUsuario datosUsuario;

}