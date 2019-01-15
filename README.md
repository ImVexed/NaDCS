# NaDCS
Not a Distributed Computing System, a rapidly deployable and highly scaleable distributed computing system for .Net Core that supports runtime dynamic code loading & unloading code using Docker.

## Try
To get a feel for the project, you can spin it up right out of the box by deploying it to a local docker swarm and running the client example.
```bash
docker-compose build --parallel
docker swarm init
docker stack deploy -c docker-compose.yml nadcs
```
Build both Client & WorkToDo in the example folder. Then copy WorkToDo.dll into the same directory as Client.dll and start the Client project.
## Demo
https://www.youtube.com/watch?v=-SgpyHsZa1U
## Planned Features
 - Prometheus metrics
 - Unloading of assemblies (See https://github.com/dotnet/coreclr/projects/9#card-13372338)
 - Cleanup of how Taskable code is invoked.

## So What Does It Do?
NaDCS [nɑ dɪks] intends to be a highly scaleable and rapidly deployable distributed computing system.
First I'd like to establish some terms you'll see in the code a lot. 

 - Node: A singular end-point that works to compute the requests submitted
 - Task: A singular request that will be distributed to a node to be computed
 - Scheduler: The central host that orchestrates all the Nodes and ballances the Tasks between them.
 - Client: The program or logic that submits Tasks to the Scheduler
 
The system intends to take on a star topology (atleast logically), and is simple in concept. A client, or any piece of arbitrary software
is built but needs to execute a lot of complex code that takes too much time for it to complete on it's own, the solution is obvious but
talk is easy. 

This is where NaDCS comes in.

NaDCS's Node and Scheduler system will never need to be modified by the average user or developer, both are pre-built containerized
images ready to go with only a few command line arguments seperating you from a working cluster of computing nodes. The only work that needs to be done is in the taskable code. Any code to be distributed to the nodes needs to be transplanted into a library that 
implements NLC functions similar to a SharedClass. And thats really it. After that it's a breeze to orchestrate tasks with the Scheduler from the Client 
using the provided library, the taskable code will be sent to the Scheduler and will then be distributed to the Nodes and dynamically 
loaded and prepared for Tasking. Each node will be notified and passed contextual parameters unique to that specific Task when the 
Scheduler decides to task the node. You only have to write the code once.
