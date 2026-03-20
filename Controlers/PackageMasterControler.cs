using Godot;
using System;
using System.Collections.Generic;

public partial class PackageMasterControler : Node3D
{
	List<PackageComponent> sourceNodes = new List<PackageComponent>();
	List<Node3D> destinationNodes = new List<Node3D>();

	[Export] Node3D sourceNodesRoot;
	[Export] Node3D destinationNodesRoot;
	public bool packageActive = false;

	public PackageComponent activePackageSource;
	public Node3D activePackageDestination;
	Random rng = new Random();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node3D node in sourceNodesRoot.GetChildren())
		{
			PackageComponent component = node.GetNode("PackageComponent") as PackageComponent;
			sourceNodes.Add(component);
		}
		foreach(Node3D node in destinationNodesRoot.GetChildren())
		{
			destinationNodes.Add(node);
		}
		GD.Print("bazinga");
        if (packageActive == false)
        {
            int i = rng.Next() % sourceNodes.Count;
            int j = rng.Next() % destinationNodes.Count;
            activePackageSource = sourceNodes[i];
            activePackageDestination = destinationNodes[j];
            activePackageSource.hasPackage = true;
            packageActive = true;
            GD.Print("Source: " + activePackageSource.GetParent().Name + "; destination: " + activePackageDestination.Name);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
    public override void _PhysicsProcess(double delta)
    {
		if (packageActive == false)
		{
            int i = rng.Next() % sourceNodes.Count;
            int j = rng.Next() % destinationNodes.Count;
            activePackageSource = sourceNodes[i];
            activePackageDestination = destinationNodes[j];
            activePackageSource.hasPackage = true;
            packageActive = true;
            GD.Print("Source: " + activePackageSource.GetParent().Name + "; destination: " + activePackageDestination.Name);
        }
		//else
		//{
		//	GD.Print("Source: " + activePackageSource.Name +"; destination: " + activePackageDestination.Name);
		//}
    }
}
