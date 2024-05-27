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

public abstract class World : MonoBehaviour
{
    public virtual void OnInitGenepool()
    { }

    public virtual void OnStartPopulation()
    { }

    public virtual void OnPreEvaluateGeneration()
    { }

    public virtual void OnPostEvaluateGeneration()
    { }

    public abstract Agent CreateAgent(Genome genome);
}

public class Genepool : MonoBehaviour
{
    public int Generation { get; private set; } = 0;
    public bool IsGenepoolInitialized { get; private set; } = false;
    public bool IsPopulationEvaluated { get; private set; } = false;

    [ContextMenu("Init Genepool")]
    public void InitGenepool()
    {
        ClearAgents();

        // Initialize with agents with random genomes
        agents = new List<Agent>();
        for (int i = 0; i < populationSize; i++) agents.Add(world.CreateAgent(null));
        Generation = 0;
        IsGenepoolInitialized = true;

        Debug.Log("Genepool initialized with " + populationSize + " agents");
        world.OnInitGenepool();
        world.OnStartPopulation();
    }

    public void EvaluatePopulation()
    {
        if (!IsGenepoolInitialized) return;
        if (IsPopulationEvaluated) return;

        world.OnPreEvaluateGeneration();

        // Evaluate all agents in the population
        IsPopulationEvaluated = true;
        foreach (Agent agent in agents)
        {
            agent.StepEvaluation();
            if (!agent.HasEvaluated()) IsPopulationEvaluated = false;
        }

        if (IsPopulationEvaluated) Debug.Log("Generation " + Generation + " evaluated");

        world.OnPostEvaluateGeneration();
    }

    public void IterateGeneration()
    {
        if (!IsGenepoolInitialized) return;
        if (!IsPopulationEvaluated) return;

        // Sort agents by fitness
        agents.Sort((a, b) => a.GetFitness().CompareTo(b.GetFitness()));
        Debug.Log("Generation " + Generation + " best fitness: " + agents[agents.Count - 1].GetFitness());

        // Create new population with best agent
        List<Genome> nextGenomes = new List<Genome>();
        nextGenomes.Add(agents[agents.Count - 1].GetGenome());

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

        world.OnStartPopulation();
    }

    [Header("Genepool Settings")]
    [SerializeField] private int populationSize;
    [SerializeField] private float mutationRate;
    [SerializeField] private World world;

    [Header("Control Panel")]
    [SerializeField] private bool toEvaluate;
    [SerializeField] private bool toQuickEvaluate;
    [SerializeField] private bool toIterate;

    private List<Agent> agents;

    private void Update()
    {
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

    protected static List<Agent> SelectTournament(List<Agent> agents, int tournamentSize, int count)
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
