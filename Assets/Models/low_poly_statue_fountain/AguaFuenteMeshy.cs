using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class AguaFuenteMeshy : MonoBehaviour
{
    [SerializeField] private MeshFilter modelo;
    [SerializeField] private Material materialAgua;
    [SerializeField] private Material materialEspuma;
    [SerializeField] private float giroFrente;
    [SerializeField] private bool chorrosLaterales = true;
    [SerializeField] private bool vistaPrevia = true;
    [SerializeField, Min(0f)] private float velocidad = 1f;

    private const float Nivel = -0.363f;
    private const int Tramos = 18;
    private const int Lados = 6;

    // Perímetro interior del estanque medido en el FBX cada 7.5 grados.
    private static readonly float[] Radios = {
        .3165f, .3188f, .3262f, .3271f, .3209f, .3223f, .3316f, .3263f,
        .3240f, .3270f, .3356f, .3270f, .3243f, .3270f, .3356f, .3275f,
        .3243f, .3269f, .3330f, .3230f, .3225f, .3273f, .3250f, .3177f,
        .3159f, .3195f, .3218f, .3145f, .3127f, .3161f, .3180f, .3124f,
        .3123f, .3167f, .3244f, .2936f, .2911f, .2936f, .3244f, .3166f,
        .3112f, .3117f, .3179f, .3158f, .3123f, .3141f, .3213f, .3195f
    };

    private static readonly float[] RadiosVasija = {
        .0208f, .0219f, .0224f, .0222f, .0232f, .0252f,
        .0245f, .0241f, .0220f, .0190f, .0191f, .0210f
    };

    private readonly List<Vector3> vertices = new List<Vector3>(660);
    private readonly List<Vector2> uv = new List<Vector2>(660);
    private readonly List<Color32> colores = new List<Color32>(660);
    private readonly List<int> agua = new List<int>(2200);
    private readonly List<int> espuma = new List<int>(1200);

    private GameObject efecto;
    private MeshRenderer rendererAgua;
    private Mesh malla;
    private Material aguaAsignada;
    private Material espumaAsignada;
    private bool mallaPendiente = true;
    private bool avisoMostrado;
    private float tiempo;

    private void CrearEfecto()
    {
        const HideFlags temporales = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
        efecto = new GameObject("Agua animada de la fuente", typeof(MeshFilter), typeof(MeshRenderer));
        efecto.hideFlags = temporales;
        efecto.transform.SetParent(transform, false);
        rendererAgua = efecto.GetComponent<MeshRenderer>();
        rendererAgua.shadowCastingMode = ShadowCastingMode.Off;
        rendererAgua.receiveShadows = false;

        malla = new Mesh { name = "Agua de la fuente Meshy", hideFlags = temporales };
        malla.MarkDynamic();
        efecto.GetComponent<MeshFilter>().sharedMesh = malla;
        mallaPendiente = true;
    }

    private bool MedirModelo(out Vector3 centro, out float escala)
    {
        centro = Vector3.zero;
        escala = 0f;
        if (modelo == null || modelo.sharedMesh == null)
            return false;

        Bounds b = modelo.sharedMesh.bounds;
        Matrix4x4 aRaiz = transform.worldToLocalMatrix * modelo.transform.localToWorldMatrix;
        centro = aRaiz.MultiplyPoint3x4(b.center);
        Vector3 mitad = b.extents;
        float alto = 2f * (Mathf.Abs(aRaiz.m10) * mitad.x
            + Mathf.Abs(aRaiz.m11) * mitad.y + Mathf.Abs(aRaiz.m12) * mitad.z);
        escala = alto / .953125f;
        return escala > .00001f;
    }

    private void OnValidate()
    {
        mallaPendiente = true;
    }

    private void Update()
    {
        bool enJuego = Application.IsPlaying(gameObject);
        if (!enJuego && (!vistaPrevia || !gameObject.scene.IsValid()))
        {
            LiberarEfecto();
            return;
        }

        if (!MedirModelo(out Vector3 centro, out float escala) || materialAgua == null)
        {
            LiberarEfecto();
            if (enJuego && !avisoMostrado)
            {
                Debug.LogWarning("Asigna el Mesh Filter de SecondStatue y Material Agua.", this);
                avisoMostrado = true;
            }
            return;
        }

        avisoMostrado = false;
        if (efecto == null || malla == null)
        {
            LiberarEfecto();
            CrearEfecto();
        }

        Transform marco = efecto.transform;
        Quaternion giro = Quaternion.Euler(0f, giroFrente, 0f);
        Vector3 escalaLocal = Vector3.one * escala;
        if (marco.localPosition != centro) marco.localPosition = centro;
        if (marco.localRotation != giro) marco.localRotation = giro;
        if (marco.localScale != escalaLocal) marco.localScale = escalaLocal;
        if (efecto.layer != modelo.gameObject.layer) efecto.layer = modelo.gameObject.layer;

        Material materialOndas = materialEspuma != null ? materialEspuma : materialAgua;
        if (aguaAsignada != materialAgua || espumaAsignada != materialOndas)
        {
            rendererAgua.sharedMaterials = new[] { materialAgua, materialOndas };
            aguaAsignada = materialAgua;
            espumaAsignada = materialOndas;
        }

        if (enJuego)
        {
            tiempo += Time.deltaTime * velocidad;
            Dibujar();
        }
        else if (mallaPendiente)
        {
            tiempo = 0.23f;
            Dibujar();
        }

        mallaPendiente = false;
    }

    private void Dibujar()
    {
        vertices.Clear();
        uv.Clear();
        colores.Clear();
        agua.Clear();
        espuma.Clear();

        Superficie(Punto(.00025f, Nivel, -.0027f), Radios);
        Superficie(Punto(0f, .2005f, .076f), RadiosVasija);

        Chorro(Punto(0f, .206f, .1025f), Punto(0f, .190f, .176f),
            Punto(0f, -.220f, .200f), Punto(0f, Nivel, .214f), .009f, 0f);

        if (chorrosLaterales)
        {
            for (int lado = -1; lado <= 1; lado += 2)
            {
                Chorro(Punto(lado * .132f, -.253f, .087f),
                    Punto(lado * .147f, -.248f, .153f),
                    Punto(lado * .167f, -.322f, .177f),
                    Punto(lado * .183f, Nivel, .189f), .006f, lado * .27f);
            }
        }

        malla.Clear(true);
        malla.SetVertices(vertices);
        malla.SetUVs(0, uv);
        malla.SetColors(colores);
        malla.subMeshCount = 2;
        malla.SetTriangles(agua, 0, false);
        malla.SetTriangles(espuma, 1, false);
        malla.RecalculateNormals();
        malla.RecalculateBounds();
    }

    private void Superficie(Vector3 centro, float[] radios)
    {
        int inicio = vertices.Count;
        AgregarVertice(centro, true);
        for (int i = 0; i < radios.Length; i++)
        {
            float angulo = i * Mathf.PI * 2f / radios.Length;
            AgregarVertice(centro + Circulo(angulo) * radios[i], true);
            Triangulo(agua, inicio, inicio + 1 + i, inicio + 1 + (i + 1) % radios.Length);
        }
    }

    private void Chorro(Vector3 origen, Vector3 controlA, Vector3 controlB,
        Vector3 impacto, float grosor, float fase)
    {
        int inicio = vertices.Count;
        Vector3 lateral = Vector3.Cross(Vector3.up, impacto - origen).normalized;
        for (int i = 0; i <= Tramos; i++)
        {
            float t = i / (float)Tramos;
            Vector3 centro = Bezier(origen, controlA, controlB, impacto, t);
            Vector3 eje = Bezier(origen, controlA, controlB, impacto, Mathf.Min(1f, t + .01f))
                - Bezier(origen, controlA, controlB, impacto, Mathf.Max(0f, t - .01f));
            Vector3 normal = Vector3.Cross(eje.normalized, lateral);
            float radio = grosor * (1f - .23f * t
                + .12f * Mathf.Sin(27f * t - 13f * tiempo + fase));

            for (int j = 0; j < Lados; j++)
            {
                float a = j * Mathf.PI * 2f / Lados;
                AgregarVertice(centro + radio * (lateral * Mathf.Cos(a) + normal * (.65f * Mathf.Sin(a))));
                if (i < Tramos)
                {
                    int p = inicio + i * Lados + j;
                    int q = inicio + i * Lados + (j + 1) % Lados;
                    Cuadro(agua, p, q, q + Lados, p + Lados);
                }
            }
        }

        Onda(impacto, Mathf.Repeat(tiempo * 1.7f + fase, 1f));
        Onda(impacto + Vector3.up * .0006f, Mathf.Repeat(tiempo * 1.7f + fase + .5f, 1f));
        for (int i = 0; i < 4; i++)
        {
            float t = Mathf.Repeat(tiempo * (2.1f + .17f * i) + fase + .23f * i, 1f);
            Vector3 centro = impacto + Circulo(1.8f * i) * (.025f * t)
                + Vector3.up * (.058f * t * (1f - t));
            Gota(centro, .0031f * (1f - .68f * t));
        }
    }

    private void Onda(Vector3 impacto, float t)
    {
        const int segmentos = 16;
        int inicio = vertices.Count;
        float radio = Mathf.Lerp(.006f, .043f, t);
        float ancho = Mathf.Lerp(.0021f, .0003f, t);
        Vector3 centro = impacto + Vector3.up * .0012f;

        for (int i = 0; i < segmentos; i++)
        {
            Vector3 dir = Circulo(i * Mathf.PI * 2f / segmentos);
            AgregarVertice(centro + dir * radio, esEspuma: true);
            AgregarVertice(centro + dir * (radio + ancho), esEspuma: true);
            int a = inicio + i * 2;
            int b = inicio + (i + 1) % segmentos * 2;
            Cuadro(espuma, a, a + 1, b + 1, b);
        }
    }

    private void Gota(Vector3 centro, float radio)
    {
        int inicio = vertices.Count;
        AgregarVertice(centro + Vector3.up * (radio * 1.5f), esEspuma: true);
        AgregarVertice(centro - Vector3.up * (radio * 1.5f), esEspuma: true);
        for (int i = 0; i < 4; i++)
            AgregarVertice(centro + Circulo(i * Mathf.PI * .5f) * radio, esEspuma: true);

        for (int i = 0; i < 4; i++)
        {
            int a = inicio + 2 + i;
            int b = inicio + 2 + (i + 1) % 4;
            Triangulo(espuma, inicio, a, b);
            Triangulo(espuma, inicio + 1, b, a);
        }
    }

    private void AgregarVertice(Vector3 posicion, bool superficie = false, bool esEspuma = false)
    {
        vertices.Add(posicion);
        float u = Mathf.Clamp01(posicion.x / .74f + .5f);
        float v = Mathf.Clamp01(.5f + (superficie ? posicion.z / .74f : posicion.y / .953125f));
        uv.Add(new Vector2(u, v));
        colores.Add(esEspuma ? new Color32(209, 243, 249, 236)
            : new Color32(79, 183, 210, 224));
    }

    private static Vector3 Punto(float x, float y, float frente)
    {
        // El efecto usa el centro de bounds del FBX como origen.
        return new Vector3(x - .000244f, y - .0002445f, frente + .0029295f);
    }

    private static Vector3 Circulo(float angulo)
    {
        return new Vector3(Mathf.Cos(angulo), 0f, -Mathf.Sin(angulo));
    }

    private static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1f - t;
        return a * (u * u * u) + b * (3f * u * u * t)
            + c * (3f * u * t * t) + d * (t * t * t);
    }

    private static void Triangulo(List<int> indices, int a, int b, int c)
    {
        indices.Add(a);
        indices.Add(b);
        indices.Add(c);
    }

    private static void Cuadro(List<int> indices, int a, int b, int c, int d)
    {
        Triangulo(indices, a, b, c);
        Triangulo(indices, a, c, d);
    }

    private void OnDrawGizmosSelected()
    {
        if (!MedirModelo(out Vector3 centro, out float escala))
            return;

        Quaternion giro = Quaternion.Euler(0f, giroFrente, 0f);
        Vector3 salida = centro + giro * Punto(0f, .206f, .1025f) * escala;
        Vector3 llegada = centro + giro * Punto(0f, Nivel, .214f) * escala;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(salida),
            .013f * escala * transform.lossyScale.magnitude / Mathf.Sqrt(3f));
        Gizmos.DrawLine(transform.TransformPoint(salida), transform.TransformPoint(llegada));
    }

    private void OnDisable()
    {
        LiberarEfecto();
    }

    private void LiberarEfecto()
    {
        if (efecto != null)
        {
            if (Application.IsPlaying(gameObject)) Destroy(efecto);
            else DestroyImmediate(efecto);
        }

        if (malla != null)
        {
            if (Application.IsPlaying(gameObject)) Destroy(malla);
            else DestroyImmediate(malla);
        }

        efecto = null;
        malla = null;
        rendererAgua = null;
        aguaAsignada = null;
        espumaAsignada = null;
    }
}