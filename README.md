# 3D Navigation with Octree in Unity

## Project Overview

This project introduces a 3D navigation system to the Unity game engine, which traditionally only supports navigation on surfaces (NavMesh). The system utilizes an octree data structure to discretize 3D space and implement obstacle avoidance for agents, allowing for efficient path planning in three-dimensional environments.

## Features

- **Octree Data Structure**: Discretizes 3D space into manageable cubes for efficient pathfinding.
- **Dual Graph Generation**: Converts octree data into a graph structure for path planning.
- **Path Planning Algorithms**:
  - **A***: Enhanced with a heuristic for faster pathfinding.
  - **Post-Smoothing**: Optimizes paths computed by A*.
  - **Basic Theta***: Incorporates post-smoothing during the search process.
  - **Lazy Theta***: Reduces computation time by minimizing line-of-sight checks.
- **Multi-Target Single-Source Optimization**: Efficiently calculates paths for multiple targets aiming to reach a single source.

## Objectives

1. **Octree Creation**: Efficiently discretize 3D space with an octree structure.
2. **Dual Graph Creation**: Generate a graph from the octree for path planning.
3. **Agent Path Planning**: Implement and test various path planning algorithms.
4. **Movement of Agents**: Ensure realistic movement of agents using optimized paths.

## Implementation Details

### Octree Structure

- **Recursive Subdivision**: The octree divides the space recursively into smaller cubes where obstacles are present.
- **Node Information**: Each node in the octree stores its position, size, depth, and other relevant data.
- **Collision Detection**: Uses Unity's built-in `Physics.OverlapBox` for detecting collisions.

### Path Planning

1. **A***:
   - Combines distance from the source and a heuristic to the goal.
   - Uses a priority queue for efficient node expansion.
2. **Post-Smoothing**:
   - Enhances paths by reducing unnecessary turns and optimizing length.
3. **Theta* Variants**:
   - **Basic Theta***: Checks for direct lines-of-sight to improve paths during the search.
   - **Lazy Theta***: Uses lazy evaluation to minimize line-of-sight checks, improving performance.

### Multi-Target Single-Source Optimization

- **Registration of Targets**: Targets register to the source, which sorts them based on distance and computes paths efficiently.

## Results

- **Performance**: The octree structure significantly reduces space and time complexity compared to traditional grid methods.
- **Realistic Movement**: Agents navigate in 3D space realistically using optimized paths.
- **Efficiency**: The implemented path planning algorithms perform well in various test scenarios, balancing path length and computation time.


## How to install 
- Simply add a git package in unity by copying: 
```git  
 https://github.com/JoyAlbertini/Unity-3Dnavigation 
````
- The git packahe contains all needed dependencies making it plug and play 
    - A copy of C5 (https://github.com/sestoft/C5/tree/master/C5) (if you already have installed C5 delete it in **\Runtime\Dependencies\C5**) 
    - A copy of SimpleMinMaxSlider (https://github.com/augustdominik/SimpleMinMaxSlider) fixed in order to be able to build application, 

- To use the sample scane (**ExampleScene**) you need to install **unity TextMeshPro**

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgments

- Advisor: Prof. Evanthia Papadopoulou
- References to works by Nash Alex and others on path planning algorithms and octree structures.

---
