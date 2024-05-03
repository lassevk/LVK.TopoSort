LVK.TopoSort
===

[![build](https://github.com/lassevk/LVK.TopoSort/actions/workflows/build.yml/badge.svg)](https://github.com/lassevk/LVK.TopoSort/actions/workflows/build.yml)
[![codecov](https://codecov.io/github/lassevk/LVK.TopoSort/graph/badge.svg?token=NF5T1KVQYY)](https://codecov.io/github/lassevk/LVK.TopoSort)
[![codeql](https://github.com/lassevk/LVK.TopoSort/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/lassevk/LVK.TopoSort/actions/workflows/github-code-scanning/codeql)

This library implements topological sorting, which allows you to per-item specify dependencies, like
"Item B must come after Item A", and then let the sorting algorithm figure out one correct order that satisfies
all the constraints.

Example
---

```mermaid
graph TD;
    A --> B;
    A --> C;
    B --> D;
    C --> D;
    E --> F;
    G;
    H;
```

```csharp
Constraint<string>[] constraints = [
    Constraint.Create("A", "B"),
    Constraint.Create("A", "C"),
    Constraint.Create("B", "D"),
    Constraint.Create("C", "D"),
    Constraint.Create("E", "F"),
    Constraint.CreateUnconstrained("G"),
    Constraint.CreateUnconstrained("H"),
];

foreach (var grp in constraints.OrderedGroups())
    Console.WriteLine(string.Join(", ", grp));
```

One correct output from this would be:

```text
A, E, G, H
B, C, F
D
``` 

The `OrderedGroups` method will return groups of elements, where each group has no intra-dependencies.
In the example above, there are no dependencies between `A`, `E`, `G`, and `H`.

The `OrderedGroups` takes an optional `IComparer<T>` comparer that for each such group orders the individual
elements in the group. By default the `Comparer<T>.Default` is used.

There is also another method, `Ordered` that expands these groups, with the exact same constraints,
the following code:

```csharp
foreach (var element in constraints.Ordered())
    Console.WriteLine(element);
```

Would output this:

```text
A
E
G
H
B
C
F
D
``` 

The same elements, just as individual elements and not grouped together.

Cycles
---
If the constraint graph contains cycles, an exception will be thrown during enumeration.

Example:

```mermaid
graph LR;
    A --> B;
    B --> C;
    C --> D;
    D --> B;
```

```csharp
Constraint<string>[] constraints = [
    Constraint.Create("A", "B"),
    Constraint.Create("B", "C"),
    Constraint.Create("C", "D"),
    Constraint.Create("D", "B"),
];

foreach (var grp in constraints.OrderedGroups())
    Console.WriteLine(string.Join(", ", grp));
```

This will throw an `TopologicalCycleException`. There is a property on this exception, `Elements`, that contains
the elements still in the graph at the point where the cycle was detected, which in the above case would be all the
elements except `A`.
