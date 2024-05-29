using System.Collections.Generic;
using UnityEngine;

public abstract class Genome
{
    public abstract Genome Crossover(Genome other);

    public abstract void Mutate(float mutationRate);
}

public abstract class Agent : MonoBehaviour
{
    public abstract void StepEvaluation();

    public abstract bool HasEvaluated();

    public abstract float GetFitness();

    public abstract Genome GetGenome();
}

public interface IGenepoolListener
{
    void OnInitGenepool();

    void OnStartPopulation();

    void OnPreEvaluateGeneration();

    void OnPostEvaluateGeneration();

    void OnEndGeneration();
}

public abstract class World : MonoBehaviour, IGenepoolListener
{
    public virtual void OnInitGenepool()
    { }

    public virtual void OnStartPopulation()
    { }

    public virtual void OnPostEvaluateGeneration()
    { }

    public virtual void OnPreEvaluateGeneration()
    { }

    public virtual void OnEndGeneration()
    { }

    public abstract Agent CreateAgent(Genome genome);
}

public class Genepool : MonoBehaviour
{
    public string PopulationSize
    {
        get => populationSize.ToString();
        set
        {
            if (int.TryParse(value, out int result)) populationSize = result;
            else populationSize = 0;
        }
    }

    public int Generation { get; private set; } = 0;
    public float BestFitness { get; private set; } = 0.0f;
    public bool ToEvaluate { get => toEvaluate; set => toEvaluate = value; }
    public bool ToQuickEvaluate { get => toQuickEvaluate; set => toQuickEvaluate = value; }
    public bool ToIterate { get => toIterate; set => toIterate = value; }

    public void InitGenepool()
    {
        ClearAgents();

        // Initialize with agents with random genomes
        agents = new List<Agent>();
        for (int i = 0; i < populationSize; i++) agents.Add(world.CreateAgent(null));
        Generation = 0;
        isGenepoolInitialized = true;

        AddListener(world);
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnInitGenepool();
            listeners[i].OnStartPopulation();
        }
    }

    public void EvaluatePopulation()
    {
        if (!isGenepoolInitialized) return;
        if (IsPopulationEvaluated) return;

        for (int i = 0; i < listeners.Count; i++) listeners[i].OnPreEvaluateGeneration();

        // Evaluate all agents in the population
        IsPopulationEvaluated = true;
        foreach (Agent agent in agents)
        {
            agent.StepEvaluation();
            if (!agent.HasEvaluated()) IsPopulationEvaluated = false;
        }

        for (int i = 0; i < listeners.Count; i++) listeners[i].OnPostEvaluateGeneration();
    }

    public void IterateGeneration()
    {
        if (!isGenepoolInitialized) return;
        if (!IsPopulationEvaluated) return;

        // Sort agents by fitness
        agents.Sort((a, b) => a.GetFitness().CompareTo(b.GetFitness()));

        // Create new population with best agent
        List<Genome> nextGenomes = new List<Genome>();
        nextGenomes.Add(agents[agents.Count - 1].GetGenome());
        BestFitness = agents[agents.Count - 1].GetFitness();

        // Select parents for crossover
        int childCount = populationSize - 1;
        List<Agent> parents = SelectTournament(agents, 3, childCount * 2);

        // Crossover and mutate parents
        for (int i = 0; i < childCount; i++)
        {
            Genome parent1 = parents[i * 2 + 0].GetGenome();
            Genome parent2 = parents[i * 2 + 1].GetGenome();
            Genome childGenome = parent1.Crossover(parent2);
            childGenome.Mutate(mutationRate);
            nextGenomes.Add(childGenome);
        }

        // Create new population
        ClearAgents();
        for (int i = 0; i < populationSize; i++) agents.Add(world.CreateAgent(nextGenomes[i]));
        Generation++;
        IsPopulationEvaluated = false;

        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEndGeneration();
            listeners[i].OnStartPopulation();
        }
    }

    public void AddListener(IGenepoolListener listener)
    {
        if (listeners == null) listeners = new List<IGenepoolListener>();
        listeners.Add(listener);
    }

    [Header("Genepool Settings")]
    [SerializeField] private int populationSize;
    [SerializeField] private float mutationRate;
    [SerializeField] private World world;

    [Header("Control Panel")]
    [SerializeField] private bool toEvaluate;
    [SerializeField] private bool toQuickEvaluate;
    [SerializeField] private bool toIterate;

    private bool isGenepoolInitialized = false;
    private bool IsPopulationEvaluated = false;
    private List<Agent> agents;
    private List<IGenepoolListener> listeners;

    private void Start()
    {
        InitGenepool();
    }

    private void Update()
    {
        if (!isGenepoolInitialized) return;

        if (toEvaluate)
        {
            if (toQuickEvaluate)
            {
                while (!IsPopulationEvaluated) EvaluatePopulation();
            }
            else EvaluatePopulation();
        }

        if (toIterate && IsPopulationEvaluated)
        {
            IterateGeneration();
        }
    }

    private void ClearAgents()
    {
        if (agents == null) return;
        foreach (Agent agent in agents) Destroy(agent.gameObject);
        agents.Clear();
    }

    private static List<Agent> SelectTournament(List<Agent> agents, int tournamentSize, int count)
    {
        List<Agent> selected = new List<Agent>();
        for (int i = 0; i < count; i++)
        {
            List<Agent> tournament = new List<Agent>();
            for (int j = 0; j < tournamentSize; j++)
            {
                tournament.Add(agents[Random.Range(0, agents.Count)]);
            }
            tournament.Sort((a, b) => a.GetFitness().CompareTo(b.GetFitness()));
            selected.Add(tournament[tournament.Count - 1]);
        }

        return selected;
    }
}
