using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class CaminataNPC : MonoBehaviour
{
    [SerializeField] private Transform[] puntos;
    [SerializeField, Min(0f)] private float velocidad = 1.2f;
    [SerializeField, Min(0f)] private float velocidadGiro = 360f;
    [SerializeField, Min(0f)] private float esperaEnPunto = 2f;

    private CharacterController controlador;
    private Animator animator;
    private float velocidadAnimacion;
    private float esperaRestante;
    private int puntoActual;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        velocidadAnimacion = animator.speed;
    }

    private void Update()
    {
        if (puntos == null || puntos.Length == 0 || Time.deltaTime == 0f)
            return;

        if (esperaRestante > 0f)
        {
            esperaRestante -= Time.deltaTime;
            return;
        }

        Transform punto = puntos[puntoActual];
        if (punto == null)
        {
            AvanzarAlSiguientePunto();
            return;
        }

        Vector3 inicio = transform.position;
        Vector3 direccion = punto.position - inicio;
        direccion.y = 0f;

        if (direccion.sqrMagnitude > 0.0025f)
        {
            Quaternion giro = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, giro, velocidadGiro * Time.deltaTime);

            Vector3 paso = Vector3.ClampMagnitude(
                direccion, velocidad * Time.deltaTime);
            controlador.Move(paso);
        }

        Vector3 restante = punto.position - transform.position;
        restante.y = 0f;

        if (restante.sqrMagnitude <= 0.0025f)
        {
            esperaRestante = esperaEnPunto;
            AvanzarAlSiguientePunto();

            if (esperaRestante > 0f)
                animator.speed = 0f;

            return;
        }

        bool avanzo = (transform.position - inicio).sqrMagnitude > 0.00000001f;
        animator.speed = avanzo ? velocidadAnimacion : 0f;
    }

    private void AvanzarAlSiguientePunto()
    {
        puntoActual = (puntoActual + 1) % puntos.Length;
    }

    private void OnDisable()
    {
        if (animator != null)
            animator.speed = velocidadAnimacion;
    }
}