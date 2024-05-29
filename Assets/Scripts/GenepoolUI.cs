using UnityEngine;

public class GenepoolUI : MonoBehaviour, IGenepoolListener
{
    public void OnInitGenepool()
    {
        generationText.text = genepool.Generation.ToString();
    }

    public void OnStartPopulation()
    { }

    public void OnPostEvaluateGeneration()
    { }

    public void OnPreEvaluateGeneration()
    { }

    public void OnEndGeneration()
    {
        generationText.text = genepool.Generation.ToString();
        bestFitnessText.text = genepool.BestFitness.ToString();
    }

    [Header("References")]
    [SerializeField] private Genepool genepool;
    [SerializeField] private TMPro.TextMeshProUGUI generationText;
    [SerializeField] private TMPro.TMP_InputField populationSizeText;
    [SerializeField] private TMPro.TextMeshProUGUI bestFitnessText;

    private void Awake()
    {
        genepool.AddListener(this);
        genepool.PopulationSize = populationSizeText.text;
    }
}
