using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class AguaFuentePaloma : MonoBehaviour
{
    private static readonly int TiempoAgua = Shader.PropertyToID("_TiempoAgua");

    // Bounds del cuenco inferior del FBX, medidas en el espacio de diseño.
    private const float AnchoReferencia = 4.17f;
    private const float AltoReferencia = 0.718f;
    private const float CentroVerticalReferencia = 0.399f;

    private const int SegmentosTazon = 16;
    private const int SegmentosChorro = 16;
    private const int LadosChorro = 6;
    private const int SegmentosOnda = 12;
    private const int GotasPorChorro = 4;

    [SerializeField] private Color agua = new Color(0.09f, 0.65f, 0.74f, 0.86f);
    [SerializeField] private Color espuma = new Color(0.83f, 0.97f, 1f, 0.90f);
    [SerializeField, Min(0f)] private float velocidad = 1f;

    private readonly List<Vector3> vertices = new List<Vector3>(2500);
    private readonly List<Color> colores = new List<Color>(2500);
    private readonly List<Vector2> tiposAgua = new List<Vector2>(2500);
    private readonly List<int> indices = new List<int>(12000);
    private readonly List<Chorro> chorros = new List<Chorro>(13);
    private GameObject objetoAgua;
    private Mesh malla;
    private Material material;
    private bool construirIndices = true;
    private float reloj;

    private struct Chorro
    {
        public Vector3 direccion;
        public Vector3 lateral;
        public float origen;
        public float altura;
        public float radioDestino;
        public float nivelDestino;
        public float salto;
        public float grosor;

        public Vector3 Posicion(float t)
        {
            return direccion * Mathf.Lerp(origen, radioDestino, t)
                + Vector3.up * (Mathf.Lerp(altura, nivelDestino, t) + salto * 4f * t * (1f - t));
        }
    }

    private void Awake()
    {
        var shader = Resources.Load<Shader>("AguaFuentePaloma");
        if (shader == null)
        {
            Debug.LogError("Falta el shader AguaFuentePaloma en una carpeta Resources.", this);
            enabled = false;
            return;
        }

        if (!ObtenerMarco(out MeshFilter baseFuente, out Vector3 origen,
            out Quaternion orientacion, out Vector3 escala))
        {
            Debug.LogError("AguaFuentePaloma necesita los cuencos y la corona del FBX. "
                + "Coloca una sola copia del componente en la raíz de una fuente.", this);
            enabled = false;
            return;
        }

        if (baseFuente.transform.Find("Agua animada - Paloma") != null)
        {
            Debug.LogWarning("Esta fuente ya tiene un componente de agua activo.", this);
            enabled = false;
            return;
        }

        AgregarChorros(3, 0f, 0.286f, 2.777f, 0.423f, 2.451f, 0.029f, 0.034f);
        AgregarChorros(4, 0f, 0.728f, 2.454f, 0.914f, 1.722f, 0.036f, 0.050f);
        AgregarChorros(6, 0f, 1.271f, 1.728f, 1.545f, 0.653f, 0.058f, 0.071f);

        material = new Material(shader) { name = "Agua de la fuente" };
        malla = new Mesh { name = "Superficies y chorros de la fuente" };
        malla.MarkDynamic();

        objetoAgua = new GameObject("Agua animada - Paloma", typeof(MeshFilter), typeof(MeshRenderer));
        objetoAgua.layer = baseFuente.gameObject.layer;
        Transform marcoAgua = objetoAgua.transform;
        marcoAgua.SetParent(baseFuente.transform, false);
        marcoAgua.localPosition = origen;
        marcoAgua.localRotation = orientacion;
        marcoAgua.localScale = escala;
        objetoAgua.GetComponent<MeshFilter>().sharedMesh = malla;
        var rendererAgua = objetoAgua.GetComponent<MeshRenderer>();
        rendererAgua.sharedMaterial = material;
        rendererAgua.shadowCastingMode = ShadowCastingMode.Off;
        rendererAgua.receiveShadows = false;
        ActualizarMalla();
    }

    private bool ObtenerMarco(out MeshFilter baseFuente, out Vector3 origen,
        out Quaternion orientacion, out Vector3 escala)
    {
        baseFuente = null;
        origen = Vector3.zero;
        orientacion = Quaternion.identity;
        escala = Vector3.one;
        MeshFilter vasoSuperior = null;
        MeshFilter corona = null;
        foreach (MeshFilter filtro in GetComponentsInChildren<MeshFilter>(true))
        {
            if (filtro.sharedMesh == null)
                continue;

            switch (filtro.name)
            {
                case "Vaso_inferior":
                    if (baseFuente != null)
                        return false;
                    baseFuente = filtro;
                    break;
                case "Vaso_superior":
                    if (vasoSuperior != null)
                        return false;
                    vasoSuperior = filtro;
                    break;
                case "Vertederos_Corona":
                    if (corona != null)
                        return false;
                    corona = filtro;
                    break;
            }
        }

        if (baseFuente == null || vasoSuperior == null || corona == null)
            return false;

        Transform marco = baseFuente.transform;
        Bounds limites = baseFuente.sharedMesh.bounds;
        Vector3 centroSuperior = marco.InverseTransformPoint(
            vasoSuperior.transform.TransformPoint(vasoSuperior.sharedMesh.bounds.center));
        Vector3 centroCorona = marco.InverseTransformPoint(
            corona.transform.TransformPoint(corona.sharedMesh.bounds.center));
        Vector3 ejeVertical = (centroSuperior - limites.center).normalized;
        // La corona tiene tres salidas y su centro de bounds queda desplazado hacia +X.
        // Esa asimetría identifica la orientación de los vertederos sin leer los vértices.
        Vector3 ejeAncho = Vector3.ProjectOnPlane(centroCorona - limites.center, ejeVertical).normalized;
        if (ejeVertical.sqrMagnitude < 0.99f || ejeAncho.sqrMagnitude < 0.99f)
            return false;

        Vector3 ejeFondo = Vector3.Cross(ejeAncho, ejeVertical).normalized;
        escala = new Vector3(
            MedidaEnEje(limites.size, ejeAncho) / AnchoReferencia,
            MedidaEnEje(limites.size, ejeVertical) / AltoReferencia,
            MedidaEnEje(limites.size, ejeFondo) / AnchoReferencia);

        if (escala.x <= 0f || escala.y <= 0f || escala.z <= 0f)
            return false;

        orientacion = Quaternion.LookRotation(ejeFondo, ejeVertical);
        origen = limites.center - ejeVertical * (CentroVerticalReferencia * escala.y);
        return true;
    }

    private static float MedidaEnEje(Vector3 tamano, Vector3 eje)
    {
        return Vector3.Dot(tamano, new Vector3(Mathf.Abs(eje.x), Mathf.Abs(eje.y), Mathf.Abs(eje.z)));
    }

    private void AgregarChorros(int cantidad, float anguloInicial, float radioInicial,
        float altura, float radioDestino, float nivelDestino, float salto, float grosor)
    {
        for (int i = 0; i < cantidad; i++)
        {
            float angulo = (anguloInicial + 360f * i / cantidad) * Mathf.Deg2Rad;
            Vector3 direccion = new Vector3(Mathf.Cos(angulo), 0f, Mathf.Sin(angulo));
            chorros.Add(new Chorro {
                direccion = direccion,
                lateral = Vector3.Cross(Vector3.up, direccion),
                origen = radioInicial,
                altura = altura,
                radioDestino = radioDestino,
                nivelDestino = nivelDestino,
                salto = salto,
                grosor = grosor
            });
        }
    }

    private void OnEnable()
    {
        if (objetoAgua != null)
            objetoAgua.SetActive(true);
    }

    private void OnDisable()
    {
        if (objetoAgua != null)
            objetoAgua.SetActive(false);
    }

    private void Update()
    {
        if (material == null)
            return;

        reloj += Time.deltaTime * velocidad;
        material.SetFloat(TiempoAgua, reloj);
        ActualizarMalla();
    }

    private void ActualizarMalla()
    {
        vertices.Clear();
        colores.Clear();
        DibujarSuperficie(1.820f, 0.648f);
        DibujarSuperficie(1.078f, 1.717f);
        DibujarSuperficie(0.577f, 2.446f);

        for (int i = 0; i < chorros.Count; i++)
        {
            Chorro chorro = chorros[i];
            float fase = i * 0.618034f;
            DibujarChorro(chorro, fase);
            Vector3 impacto = chorro.Posicion(1f);
            DibujarOnda(impacto, Mathf.Repeat(reloj * 1.25f + fase, 1f));
            DibujarOnda(impacto + Vector3.up * 0.001f, Mathf.Repeat(reloj * 1.25f + fase + 0.5f, 1f));
            DibujarGotas(chorro, impacto, fase);
        }

        malla.SetVertices(vertices);
        malla.SetColors(colores);
        if (construirIndices)
        {
            malla.SetUVs(0, tiposAgua);
            malla.SetTriangles(indices, 0, false);
            malla.bounds = new Bounds(new Vector3(0f, 1.43f, 0f), new Vector3(4f, 2.93f, 4f));
            construirIndices = false;
        }
    }

    private void DibujarSuperficie(float radio, float altura)
    {
        int inicio = vertices.Count;
        Vector3 centro = Vector3.up * altura;
        AgregarVertice(centro, agua, true);
        Color orilla = Color.Lerp(agua, espuma, 0.15f);

        for (int i = 0; i < SegmentosTazon; i++)
        {
            float angulo = i * Mathf.PI * 2f / SegmentosTazon;
            Vector3 borde = new Vector3(Mathf.Cos(angulo), 0f, Mathf.Sin(angulo)) * radio;
            AgregarVertice(centro + borde, orilla, true);
            Triangulo(inicio, inicio + 1 + (i + 1) % SegmentosTazon, inicio + 1 + i);
        }
    }

    private void DibujarChorro(Chorro chorro, float fase)
    {
        int inicio = vertices.Count;
        for (int tramo = 0; tramo <= SegmentosChorro; tramo++)
        {
            float t = tramo / (float)SegmentosChorro;
            Vector3 eje = (chorro.Posicion(t + 0.005f) - chorro.Posicion(t - 0.005f)).normalized;
            Vector3 normal = Vector3.Cross(eje, chorro.lateral).normalized;
            float pulso = 0.5f + 0.5f * Mathf.Sin(31f * t - 12f * reloj + fase * 6f);
            float grosor = chorro.grosor * (0.95f - 0.20f * t + 0.13f * pulso);
            Vector3 centro = chorro.Posicion(t) + chorro.lateral * (0.004f * Mathf.Sin(t * 17f - reloj * 7f + fase) * Mathf.Sin(Mathf.PI * t));

            for (int lado = 0; lado < LadosChorro; lado++)
            {
                float angulo = lado * Mathf.PI * 2f / LadosChorro;
                Vector3 offset = (normal * (0.30f * Mathf.Cos(angulo)) + chorro.lateral * Mathf.Sin(angulo)) * grosor;
                Color tono = Color.Lerp(agua, espuma, 0.28f + 0.36f * pulso + (lado < 2 ? 0.18f : 0f));
                AgregarVertice(centro + offset, tono);

                if (tramo < SegmentosChorro)
                {
                    int a = inicio + tramo * LadosChorro + lado;
                    int b = inicio + tramo * LadosChorro + (lado + 1) % LadosChorro;
                    Cuadrado(a, b, b + LadosChorro, a + LadosChorro);
                }
            }
        }
    }

    private void DibujarOnda(Vector3 centro, float t)
    {
        int inicio = vertices.Count;
        Color tono = espuma;
        tono.a *= 0.58f * (1f - t);
        float radio = Mathf.Lerp(0.018f, 0.105f, t);
        float ancho = Mathf.Lerp(0.012f, 0.004f, t);

        for (int i = 0; i < SegmentosOnda; i++)
        {
            float a = i * Mathf.PI * 2f / SegmentosOnda;
            Vector3 rayo = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
            AgregarVertice(centro + rayo * radio, tono);
            AgregarVertice(centro + rayo * (radio + ancho), tono);
            int interior = inicio + i * 2;
            int siguiente = inicio + ((i + 1) % SegmentosOnda) * 2;
            Cuadrado(interior, interior + 1, siguiente + 1, siguiente);
        }
    }

    private void DibujarGotas(Chorro chorro, Vector3 impacto, float fase)
    {
        for (int i = 0; i < GotasPorChorro; i++)
        {
            float t = Mathf.Repeat(reloj * (1.65f + i * 0.24f) + fase + i * 0.27f, 1f);
            float lateral = (i - 1.5f) * 0.038f;
            Vector3 centro = impacto + (chorro.lateral * lateral + chorro.direccion * 0.066f) * t;
            centro.y += (0.25f + i * 0.05f) * t * (1f - t);
            float radio = Mathf.Lerp(0.014f, 0.004f, t);
            Color tono = espuma;
            tono.a *= Mathf.Clamp01((1f - t) * 3f);
            int inicio = vertices.Count;
            AgregarVertice(centro + Vector3.up * radio * 1.45f, tono);
            AgregarVertice(centro + Vector3.down * radio * 1.45f, tono);
            AgregarVertice(centro + Vector3.right * radio, tono);
            AgregarVertice(centro + Vector3.forward * radio, tono);
            AgregarVertice(centro + Vector3.left * radio, tono);
            AgregarVertice(centro + Vector3.back * radio, tono);
            for (int lado = 0; lado < 4; lado++)
            {
                int a = inicio + 2 + lado;
                int b = inicio + 2 + (lado + 1) % 4;
                Triangulo(inicio, b, a);
                Triangulo(inicio + 1, a, b);
            }
        }
    }

    private void AgregarVertice(Vector3 posicion, Color tono, bool superficie = false)
    {
        vertices.Add(posicion);
        colores.Add(tono);
        if (construirIndices)
            tiposAgua.Add(superficie ? Vector2.one : Vector2.zero);
    }

    private void Triangulo(int a, int b, int c)
    {
        if (!construirIndices)
            return;
        indices.Add(a);
        indices.Add(b);
        indices.Add(c);
    }

    private void Cuadrado(int a, int b, int c, int d)
    {
        Triangulo(a, b, c);
        Triangulo(a, c, d);
    }

    private void OnDestroy()
    {
        if (objetoAgua != null)
            Destroy(objetoAgua);
        if (malla != null)
            Destroy(malla);
        if (material != null)
            Destroy(material);
    }
}