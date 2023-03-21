using System.Collections.Generic;
using FactoryCore;

public class Character
{
    public World World { get; private set; }
    public Point2Int GridPosition { get; protected set; }
    public InventoryCell? Inventory => GetCell<InventoryCell>(CellType.Inventory);

    protected virtual void InitCells() { }
    protected Dictionary<CellType, Cell> Cells;

    public Character(World world)
    {
        this.World = world;
        this.Cells = new Dictionary<CellType, Cell>();
        InitCells();
    }

    public virtual void Tick(float deltaTime)
    {
        foreach (var cell in Cells.Values)
        {
            cell.Tick(deltaTime);
        }
    }

    public T GetCell<T>(CellType type) where T : Cell
    {
        if (!Cells.ContainsKey(type))
        {
            return default(T);
        }

        return (T)Cells[type];
    }

    public void SetCell(Cell cell)
    {
        Cells[cell.Type] = cell;
    }

    public bool HasCell(CellType type)
    {
        return Cells.ContainsKey(type);
    }

    public virtual void OnAddToGrid(Point2Int gridPosition)
    {
        this.GridPosition = gridPosition;
        foreach (var cell in Cells.Values)
        {
            cell.OnAddToGrid();
        }
    }

    public virtual void OnRemoveFromGrid()
    {
        foreach (var cell in Cells.Values)
        {
            cell.OnRemoveFromGrid();
        }
    }
}