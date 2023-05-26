namespace Xo.Algo.RectangleCluster.Abstractions;

public interface IRectangle
{
	int X { get; set; }
	int Y { get; set; }
	int W { get; set; }
	int H { get; set; }
	bool IsElasticW { get; init; }
	bool IsElasticH { get; init; }
	string? Uuid { get; init; }
	int GroupId { get; init; }
	bool IsOverlapX(IRectangle r);
	bool IsOverlapY(IRectangle r);
	bool IsOverlap(IRectangle r);
}

public interface IRectangleBlueprint
{
	int MinH { get; init; }
	int MinW { get; init; }
	bool IsElasticW { get; init; }
	bool IsElasticH { get; init; }
	string? Uuid { get; init; }
	int GroupId { get; init; }
}

public interface IGrid
{
	IGrid Init(IEnumerable<IRectangleBlueprint> blueprints);
	IGrid Validate();
	IGrid InitCoords();
	IGrid InflateRowWidthsToMeet();
	IGrid InflateRowHeightsToHighest();
	IGrid Print();
	IGrid IniHash();
	IEnumerable<IEnumerable<IRectangle>> Result();
	IDictionary<string, IRectangle> Hash();
}
