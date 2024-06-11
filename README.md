# Unity-Pathmaker

Developed by Gabriel Nash

A script that allows you to create paths for objects to travel on. Also works with NavMesh agents.

## Setup
+ Clone this repository into the Assets folder of your Unity project.
+ Add the Path Manager Script to the object you want to use paths.

## Path Manager Script (Component)
These are fields that you can edit in the inspector with this component.

### Paths (Array)
An array of paths. *See Path (Class)*

### Start At Position 0
A boolean. If set to true, when the `StartPath` method is called, it will teleport the object to the position of path point 0 in the list of paths.

### OnPathStart
A Unity Event that is called when the object starts walking the path.

### OnPathPause
A Unity Event that is called when the path is paused.

### OnPathStop
A Unity Event that is called when the path is stopped.

### OnPathEnd
A Unity Event that is called when the object reached the position of the last path point in the array of paths.

## Public Methods
### StartPath

`StartPath()`

When called, the object starts walking on the paths.

### PausePath

`PausePath(bool isPaused)`

When called, the object will stop walking on the path. Use this method if you are going to continue the path at some point.

### StopPath

`StopPath(bool resetPaths)`

When called, the object will stop walking on the path. Use this method if you will no longer use that path, or want to reset the path based on what you set `resetPaths` to.

### GoToNextPathPoint

`GoToNextPathPoint()`

When called, the object will set the next path point in the list as its destination.

### GoToPathPoint

`GoToPathPoint(int pathNumber)`

When called, the object will set its destination to the path point index in the paths list based on `pathNumber`.

### ResetPaths

`ResetPaths(bool startAfterReset)`

When called, the path points that are using wait times will have their values reset. When `startAfterReset` is true, the `StartPath` method is called and the object with this component will start the path over.

## Path (Class)
This class contains all the properties of each path position in the list of paths inside the Path Manager Script.

### PathPoint
A transform position in the path that the object will move to. An object will move towards a point in the order the path points are indexed in the paths array. *See Paths (Array) in Path Manager Script Component.*

### TravelSpeed
A float value that tells how fast the object will move from the previous point to the next point in the path multiplied by `Time.deltaTime`.

### WaitBeforeNextPoint
A boolean that if true, will cause the object to wait for a certain period of time based on the point's `WaitTime` value.

### WaitTime
A float value. Set this only if `WaitBeforeNextPoint` is true. This is the amount of time in seconds the object will wait before it continues to the next point in the path.

### OnHitPoint
A Unity Event that is triggered the moment the object arrives at the `PathPoint` position its moving towards.