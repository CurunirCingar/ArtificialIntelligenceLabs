using UnityEngine;

public class GUIName : MonoBehaviour
{
    // имя объекта для вывода
    public string objectName;
    // использовать ли рейкастинг для отображения имени только если объект не закрыт другим объектом
    // будет работать если на объекте есть коллайдер
    public bool useRayCast;
    // размер объекта (используется в условии по рейкастингу)
    public float objectSize = 2;

    // вспомогательные переменные
    private bool _showName;
    private Vector2 _position;

    public GUISkin m_skin;

    public void Awake()
    {
        // если имя не указано, то отображаем имя объекта сцены
        if (string.IsNullOrEmpty(objectName))
        {
            objectName = name;
        }
    }

    public void Update()
    {
        _showName = false;
        // позиция относительно камеры
        Vector3 cameraRelative = Camera.main.transform.InverseTransformPoint(transform.position);
        // если z>0, то точка находится перед камерой
        if (cameraRelative.z > 0)
        {
            // если используем рейкастинг
            if (useRayCast)
            {
                RaycastHit hit;

                // направление луча
                Vector3 direction = transform.position - Camera.main.transform.position;

                // сам луч
                Ray ray = new Ray(Camera.main.transform.position, direction);

                // посылаем луч
                if (Physics.Raycast(ray, out hit))
                {
                    // если дистанция до цели удовлетворяет условиям, то отображаем имя
                    if (hit.distance >= (direction.magnitude - objectSize))
                    {
                        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                        _position = new Vector2(screenPosition.x - 60f, Screen.height - screenPosition.y - 10f);
                        _showName = true;
                    }
                }
            }
            else
            {
                // случай без рейкастинга
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                _position = new Vector2(screenPosition.x - 60f, Screen.height - screenPosition.y - 10f);
                _showName = true;
            }

        }
    }


    public void OnGUI()
    {
        GUI.skin = m_skin;
        // если следует отобразить имя
        if (_showName)
        {
            // считаем позицию
            Rect rect = new Rect(_position.x, _position.y, 120f, 20f);

            // создаем стиль с выравниванием по центру
            GUIStyle label = new GUIStyle(GUI.skin.label);

            // выводим имя объекта с созданным стилем, чтобы имя было выведено по центру
            GUI.Label(rect, objectName, label);
        }
    }
}