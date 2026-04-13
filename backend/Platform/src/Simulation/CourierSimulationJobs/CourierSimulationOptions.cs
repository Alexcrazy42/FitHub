namespace FitHub.Simulation.CourierSimulationJobs;

public class CourierSimulationOptions
{
    public const string SectionName = "CourierSimulation";

    public string AssignmentDecision { get; set; } = "accept";

    public int DecisionDelaySeconds { get; set; } = 2;

    public string[] CourierNames { get; set; } =
    [
        "Debug courier 1",
        "Debug courier 2",
        "Debug courier 3"
    ];
}
