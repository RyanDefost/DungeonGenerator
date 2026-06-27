using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InterfaceInteraction : MonoBehaviour
{
    [SerializeField] private Generator generator;
    [SerializeField] private Slider seedSlider;
    [SerializeField] private TextMeshProUGUI seedText;
    
    private void Start() => this.generator = FindAnyObjectByType<Generator>();
    private void Update() => this.seedText.text = this.seedSlider.value.ToString();

    public void Generate() => this.generator.Generate((int)this.seedSlider.value);
    public void GenerateRandom()
    {
        int randomValue = Random.Range(0, 9999);
        print(randomValue);
        this.seedSlider.value = randomValue;
        
        this.generator.Generate(randomValue);
    }
}
