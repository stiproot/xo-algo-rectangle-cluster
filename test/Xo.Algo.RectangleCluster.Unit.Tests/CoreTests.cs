namespace Xo.Algo.RectangleCluster.Unit.Tests;

public class CoreTests
{
	private readonly Random _random = new Random();
	private int _randomInt(int min = 1, int max = 5) => this._random.Next(min, max);
	private bool _randomBit => this._random.Next(0, 2) % 2 == 0 ? false : true;

	private IEnumerable<IRectangleBlueprint> RandomBlueprintFactory(int n = 15)
	{
		IRectangleBlueprint Blueprint(int groupId = 0)
			=> (IRectangleBlueprint)new RectangleBlueprint { MinW = this._randomInt(), MinH = this._randomInt(), IsElasticH = this._randomBit, IsElasticW = this._randomBit, GroupId = groupId };

		return Enumerable.Range(0, n).Select(_ => Blueprint());
	}

	// no rectangles overlap each other...
	[Test]
	public void GIVEN_AsymmetricalGrid_WHEN_Inspected_THEN_NoRectanglesShareXYCoords()
	{
		// Arrange
		var blueprints = this.RandomBlueprintFactory();

		// Act
		var grid = new Grid(15, 15)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InflateRowHeightsToHighest()
			.InitCoords()
			.Print();

		// Assert
		Assert.IsTrue(grid.Result().SelectMany(row => row).GroupBy(r => $"({r.X},{r.Y})").All(g => g.Count() == 1));
	}

	[Test]
	public void GIVEN_AsymmetricalGrid_WHEN_Inspected_THEN_NoRectanglesOverlap()
	{
		// Arrange
		var blueprints = this.RandomBlueprintFactory();

		// Act
		var grid = new Grid(15, 15)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InflateRowHeightsToHighest()
			.InitCoords()
			.Print();

		// Assert
		Assert.DoesNotThrow(() => grid.Validate());
	}

	// rectangles stay within boundry of grid...
	// (elastic w) rectangles do not exceed horizontal expansion limit... 
	[Test]
	public void GIVEN_AsymmetricalGrid_WHEN_Inspected_THEN_RectanglesDoNotExceedGridBoundary()
	{
		// Arrange
		var blueprints = this.RandomBlueprintFactory();

		// Act
		var grid = new Grid(15, 15)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InflateRowHeightsToHighest()
			.InitCoords()
			.Print();

		// Assert
		Assert.IsFalse(grid.Result().Any(row => row.Sum(r => r.W) > 15));
	}

	// (elastic w) all elastic h recangles in row expand horizontally to fill horizontal grid boundry...
	[Test]
	public void GIVEN_SymmetricalGrid_WHEN_ComposedOfElasticWidth_THEN_RectanglesExpandToWidthOfGrid()
	{
		// Arrange
		var blueprints = new List<IRectangleBlueprint>();
		blueprints.AddRange(Enumerable.Range(0, 3).Select(i => new RectangleBlueprint { MinH = 1, MinW = 3, IsElasticW = true }));

		// Act
		var grid = new Grid(15)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InitCoords()
			.Print();

		// Assert
		Assert.AreEqual(3, grid.Result().First().Count());
		Assert.AreEqual(15, grid.Result().First().Sum(r => r.W));
	}

	// (elastic h) all elastic h recangles within row expand vertically to the highest rectangle... 
	[Test]
	public void GIVEN_SymmetricalGrid_WHEN_ComposedOfElasticH_THEN_RectanglesExpandToHighest()
	{
		// Arrange
		var blueprints = new List<IRectangleBlueprint>();
		blueprints.Add(new RectangleBlueprint { MinH = 2, MinW = 3, IsElasticH = true });
		blueprints.Add(new RectangleBlueprint { MinH = 1, MinW = 3, IsElasticH = true });
		blueprints.Add(new RectangleBlueprint { MinH = 3, MinW = 3, IsElasticH = true });

		// Act
		var grid = new Grid(9)
			.Init(blueprints)
			.InflateRowHeightsToHighest()
			.InitCoords()
			.Print();

		// Assert
		Assert.AreEqual(3, grid.Result().First().Count());
		Assert.IsTrue(grid.Result().First().All(r => r.H == 3));
	}

	// rectangles groups are partitioned by rows...
	[Test]
	public void GIVEN_AsymmetricalGrid_WHEN_ComposedOfMultipleGroups_THEN_RectangleGroupsArePartitionedAsRows()
	{
		// Arrange
		var blueprints = new List<IRectangleBlueprint>();
		blueprints.AddRange(Enumerable.Range(0, 3).Select(i => new RectangleBlueprint { MinH = 1, MinW = 3, IsElasticW = true, GroupId = 0 }));
		blueprints.AddRange(Enumerable.Range(0, 2).Select(i => new RectangleBlueprint { MinH = 1, MinW = 4, IsElasticW = true, GroupId = 1 }));
		blueprints.AddRange(Enumerable.Range(0, 4).Select(i => new RectangleBlueprint { MinH = 1, MinW = 2, IsElasticW = true, GroupId = 2 }));

		// Act
		var grid = new Grid(10)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InitCoords()
			.Print();

		// Assert
		Assert.AreEqual(3, grid.Result().Count());
		Assert.AreEqual(4, grid.Result().First().First().W);
	}

	// (elastic h) rectangles of different dimensions in the same group are not partitioned, and are inflated... 
	[Test]
	public void GIVEN_AsymmetricalGrid_WHEN_ComposedOfASingleGroup_THEN_RectanglesAreNotPartitioned()
	{
		// Arrange
		var blueprints = new List<IRectangleBlueprint>();
		blueprints.AddRange(Enumerable.Range(0, 2).Select(i => new RectangleBlueprint { MinH = 1, MinW = 3, IsElasticW = true }));
		blueprints.Add(new RectangleBlueprint { MinH = 1, MinW = 4, IsElasticW = true });
		blueprints.AddRange(Enumerable.Range(0, 4).Select(i => new RectangleBlueprint { MinH = 1, MinW = 2, IsElasticW = true }));

		// Act
		var grid = new Grid(10)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InitCoords()
			.Print();

		// Assert
		Assert.AreEqual(2, grid.Result().Count());
		Assert.IsTrue(grid.Result().All(row => row.Sum(r => r.W) == 10));
	}

	// (elastic w) rectangles of different dimensions in the same group are not partitioned, and are inflated... 
	[Test]
	public void GIVEN_AsymmetricalGrid_WHEN_ComposedOfElasticHAndNon_THEN_OnlyElasticRectanglesExpand()
	{
		// Arrange
		var blueprints = new List<IRectangleBlueprint>();
		blueprints.AddRange(Enumerable.Range(0, 2).Select(i => new RectangleBlueprint { MinH = 1, MinW = 2, IsElasticW = true }));
		blueprints.Add(new RectangleBlueprint { MinH = 1, MinW = 4 });

		// Act
		var grid = new Grid(10)
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InitCoords()
			.Print();

		// Assert
		Assert.AreEqual(1, grid.Result().Count());
		Assert.AreEqual(3, grid.Result().First().First().W);
		Assert.AreEqual(4, grid.Result().First().Last().W);
		Assert.IsTrue(grid.Result().All(row => row.Sum(r => r.W) == 10));
	}

	[Test]
	public void TestHash()
	{
		static string NewGuidAsString() => Guid.NewGuid().ToString();

		// Arrange
		IGrid grid = new Grid(3);
		var blueprints = new List<IRectangleBlueprint>();
		blueprints.AddRange(Enumerable.Range(0, 3).Select(i => new RectangleBlueprint { MinH = 1, MinW = 1, Uuid = NewGuidAsString() }));

		// Act
		grid
			.Init(blueprints)
			.InflateRowWidthsToMeet()
			.InitCoords()
			.IniHash()
			.Hash();

		// Assert
		Assert.IsNotEmpty(grid.Hash());
		Assert.AreEqual(3, grid.Hash().Keys.Count());
		Assert.AreEqual(blueprints.First().Uuid, grid.Result().First().First().Uuid);
	}
}

