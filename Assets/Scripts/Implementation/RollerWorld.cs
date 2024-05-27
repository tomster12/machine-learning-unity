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

    public override void OnPostEvaluateGeneration()
    {
        // Step physics
        Physics.Simulate(Time.fixedDeltaTime);
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
    [SerializeField] private Transform target;
}
