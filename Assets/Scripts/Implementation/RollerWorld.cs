using UnityEngine;
using UnityEngine.Assertions;

public class RollerWorld : World
{
    public Vector3 Target => target.position;

    public override void OnInitGenepool()
    {
        // Stop physics
        Physics.simulationMode = SimulationMode.Script;
    }

    public override void OnStartPopulation()
    {
        // Reset time
        time = 0.0f;

        // Reset ground
        ground.transform.rotation = Quaternion.Euler(tiltX, 0, 0);
    }

    public override void OnPreEvaluateGeneration()
    {
        // Tilt ground
        ground.transform.rotation = Quaternion.Euler(tiltX, time * tiltYSpeed, 0f);
    }

    public override void OnPostEvaluateGeneration()
    {
        // Step physics
        Physics.Simulate(timeStep);
        time += timeStep;
    }

    public override Agent CreateAgent(Genome genome)
    {
        Assert.IsTrue(genome == null || genome is RollerGenome, "Genome must be of type RollerGenome");
        GameObject agentObject = Instantiate(agentPrefab);
        agentObject.transform.position = Vector3.up * 0.5f;
        RollerAgent agent = agentObject.GetComponent<RollerAgent>();
        agent.SetGenome(genome as RollerGenome);
        agent.SetWorld(this);
        return agent;
    }

    [Header("Roller World Settings")]
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private GameObject ground;
    [SerializeField] private Transform target;
    [SerializeField] private float timeStep = 0.05f;
    [SerializeField] private float tiltX = -100.0f;
    [SerializeField] private float tiltYSpeed = 90.0f;

    private float time;
}
