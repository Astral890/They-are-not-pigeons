using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class CaminataNPC : MonoBehaviour
{
    [SerializeField] private Transform[] puntos;
    [Tooltip("Escribe el orden usando el número de punto (1, 2, 3...)")]
    [SerializeField] private int[] ordenRuta = { 1, 2, 3, 2, 1, 3, 2, 1, 2, 3 };
    
    [SerializeField, Min(0f)] private float velocidad = 1.2f;
    [SerializeField, Min(0f)] private float velocidadGiro = 360f;
    [SerializeField, Min(0f)] private float esperaEnPunto = 2f;
    [SerializeField] private float gravedad = -9.81f;

    private CharacterController controlador;
    private Animator animator;
    private float velocidadAnimacion;
    private float esperaRestante;
    private float velocidadVertical;
    private int pasoActual;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        velocidadAnimacion = animator.speed;
    }

    private void Update()
    {
        if (Time.deltaTime == 0f)
            return;

        if (controlador.isGrounded && velocidadVertical < 0f)
            velocidadVertical = -2f;
        else
            velocidadVertical += gravedad * Time.deltaTime;

        Vector3 movimientoVertical = Vector3.up * (velocidadVertical * Time.deltaTime);

        if (puntos == null || puntos.Length == 0 || ordenRuta == null || ordenRuta.Length == 0 || esperaRestante > 0f)
        {
            if (esperaRestante > 0f)
                esperaRestante -= Time.deltaTime;

            controlador.Move(movimientoVertical);
            return;
        }

        // Convertimos el número de ruta (1, 2, 3...) al índice del arreglo (0, 1, 2...)
        int indicePunto = ordenRuta[pasoActual] - 1;

        if (indicePunto < 0 || indicePunto >= puntos.Length || puntos[indicePunto] == null)
        {
            AvanzarAlSiguientePunto();
            controlador.Move(movimientoVertical);
            return;
        }

        Transform punto = puntos[indicePunto];
        Vector3 inicio = transform.position;
        Vector3 direccion = punto.position - inicio;
        direccion.y = 0f;

        Vector3 pasoHorizontal = Vector3.zero;

        if (direccion.sqrMagnitude > 0.0025f)
        {
            Quaternion giro = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, giro, velocidadGiro * Time.deltaTime);

            pasoHorizontal = Vector3.ClampMagnitude(
                direccion, velocidad * Time.deltaTime);
        }

        controlador.Move(pasoHorizontal + movimientoVertical);

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

        Vector3 deltaHorizontal = transform.position - inicio;
        deltaHorizontal.y = 0f;
        bool avanzo = deltaHorizontal.sqrMagnitude > 0.00000001f;
        animator.speed = avanzo ? velocidadAnimacion : 0f;
    }

    private void AvanzarAlSiguientePunto()
    {
        pasoActual = (pasoActual + 1) % ordenRuta.Length;
    }

    private void OnDisable()
    {
        if (animator != null)
            animator.speed = velocidadAnimacion;
    }
}