using UnityEngine;
using UnityEngine.Assertions;

public class RollerGenome : Genome
{
    public Vector3 Gene(int index) => genes[index];

    public RollerGenome(Vector3[] genes)
    {
        this.genes = genes;
    }

    public RollerGenome(int geneCount)
    {
        genes = new Vector3[geneCount];
        for (int i = 0; i < geneCount; i++)
        {
            genes[i] = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f));
        }
    }

    public override Genome Crossover(Genome other)
    {
        Assert.IsTrue(other is RollerGenome, "Genomes must be of the same type to crossover");
        RollerGenome rollerOther = other as RollerGenome;

        Vector3[] newGenes = new Vector3[genes.Length];
        for (int i = 0; i < genes.Length; i++)
        {
            newGenes[i] = Random.value < 0.5f ? genes[i] : rollerOther.genes[i];
        }

        return new RollerGenome(newGenes);
    }

    public override void Mutate(float mutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                genes[i] = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f));
            }
        }
    }

    private Vector3[] genes;
}
