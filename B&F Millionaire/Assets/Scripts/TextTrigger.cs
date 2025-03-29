using UnityEngine;
using TMPro; // ��������� ������������ ��� ��� TextMeshPro

public class TextTrigger : MonoBehaviour
{
    public GameObject textPanel;
    public TMP_Text textComponent; // ������ �� ��������� ������

    [SerializeField]
    private string[] vegetableNames = // ������ � ���������� ������
    {
        "��������",
        "��������",
        "��������",
        "������",
        "�������"
    };

    private void Start()
    {
        // ������ ������ ��������� ��� ������
        if (textPanel != null)
            textPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Buyer"))
        {
            // �������� ��������� ���� � ��������� �����
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
        // ���������� ��������� ������ �������
        int randomIndex = Random.Range(0, vegetableNames.Length);
        return vegetableNames[randomIndex];
    }
}