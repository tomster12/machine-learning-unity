using UnityEngine;

public class RollerAgent : Agent
{
    public void SetWorld(RollerWorld world)
    {
        this.world = world;
    }

    public void SetGenome(RollerGenome genome)
    {
        if (genome == null) this.genome = new RollerGenome(genomeSize);
        else this.genome = genome;
    }

    public override void StepEvaluation()
    {
        if (hasEvaluated) return;

        // Apply force to target
        Vector3 direction = genome.Gene(genomeIndex);
        rb.AddForce(direction * force);
        genomeIndex++;

        // Check if target reached
        float distanceToTarget = Vector3.Distance(transform.position, world.Target);
        if (distanceToTarget < targetRadius)
        {
            fitness = rb.velocity.magnitude;
            rb.isKinematic = true;
            hasEvaluated = true;
        }

        // Check if genome is exhausted
        if (genomeIndex >= genomeSize)
        {
            fitness = 1f / genomeSize;
            rb.isKinematic = true;
            hasEvaluated = true;
        }
    }

    public override bool HasEvaluated() => hasEvaluated;

    public override float GetFitness() => fitness;

    public override Genome GetGenome() => genome;

    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Roller Agent Settings")]
    [SerializeField] private float force = 10f;
    [SerializeField] private float targetRadius = 0.5f;
    [SerializeField] private int genomeSize = 400;

    private RollerWorld world;
    private RollerGenome genome;
    private bool hasEvaluated = false;
    private float fitness = 0f;
    private int genomeIndex = 0;
}
