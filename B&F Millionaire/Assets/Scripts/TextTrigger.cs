using UnityEngine;
using TMPro; // Добавляем пространство имён для TextMeshPro

public class TextTrigger : MonoBehaviour
{
    public GameObject textPanel;
    public TMP_Text textComponent; // Ссылка на компонент текста

    [SerializeField]
    private string[] vegetableNames = // Массив с названиями овощей
    {
        "Кукуруза",
        "Картошка",
        "Баклажан",
        "Огурец",
        "Помидор"
    };

    private void Start()
    {
        // Делаем панель невидимой при старте
        if (textPanel != null)
            textPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Buyer"))
        {
            // Выбираем случайный овощ и обновляем текст
            string randomVegetable = GetRandomVegetable();
            textComponent.text = randomVegetable;

            textPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Buyer"))
        {
            textPanel.SetActive(false);
        }
    }

    private string GetRandomVegetable()
    {
        // Генерируем случайный индекс массива
        int randomIndex = Random.Range(0, vegetableNames.Length);
        return vegetableNames[randomIndex];
    }
}