# Gifs cant be posted on git, and this is not suited for git's markdown so it might look strange. Checkout https://dev.to/murilomsq/cladogenesis-in-an-engine-a-brief-study-of-how-species-are-born-4n14 to see it working


It's common to see evolution being presented as a biological tied concept, as it was its birthplace and all its mechanisms are really well described in biology, but in the last centuries we realised that evolution is an algorithm, and that means it is indifferent on what it's subject is. It will act on whatever set has some required atributes: 

**1 - Unit instances that differ from each other**
**2 - Environments that interact with the unit instances** 
**3 - These units have characteristics that influence on how they interact with the environment**
**4 - Heredity**

_It's a bit more complicated to define these attributes but these are more than enough to understand our setup_

That means evolution will act on a lot more than biological species, but yet, on stuff like culture, language, religion, music, and for us, **computation**.
The objective of this is creating a custom environment in an engine that has these traits so we can artificially see evolution concepts in practice.

## Cladogenesis/speciation 

In biology, cladogenesis can be described as a single parent species being split into two different ones:

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/45g2d00521fab9hyt2fo.png)

It happens when, for some reason, two groups of the same species stop breeding with each other, leading them to being genetically isolated. With enough isolation + being in a different environment + anagenesis (changes within the same group that offers an advantage) + time, you'll have groups so different that they won't even be able to breed with each other again, a rise of a new **_species_**. 

_Note that im using the common animalia species concept based on reprodution, just because it's more common and not to get too abstract_.

## Setup

A 3D real-time environment is really good to simulate nature concepts, so we will be using Unity engine 2020.3.12 to simulate our stuff.

#### Applying the concepts of evolution to our setup



##### 1 - Unit instances that differ from each other

That's the most important one, we gotta create a unit that is independent, and has its own traits. To do that, we create an Individual Class that contains a core variable to differentiate it from the others.
```cs
[SerializeField] private float identity;
```
This value will range from 0.1 to 1000 and its the core ID of the units. 
To represent this class in the real time 3D environment, we will have a capsule that moves around, and the speed that it moves change in respect to its identity.
```cs
speed = (identity / 10) + 1;
```

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/p3z5kzeccuorvsawke71.png)
 
We will also modify the capsule color based on the identity value.
```cs
color = new Color(identity/1000, 1 - identity/1000, 0.5f - identity/1000, 1);
```
This won't affect how they'll interact with other individuals or with the environment, it's just to make easy for us to spot the different identity values on the units.


![_800 and 200 identity values, from left to right_](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/ts0cumab32t2bc8oaiq0.png)<figcaption>800 and 200 identity values, from left to right</figcaption>

##### 2 - Environments that interact with the unit instances
 
The environments are pretty simple, a floor with walls around it to prevent the units from falling down (initially). If the player falls down it kills them.
```cs
if(goTransform.position.y <= -5) Destroy(gameObject);
```
The environment also limit the number of units inside it, so we don't rely on the system stability.

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/gk7ktgw7bihd0dib91b0.png)<figcaption>Environment</figcaption>
 
##### 3 - These units have characteristics that influence on how they interact with the environment

The way units interact with the environment is through speed only, being faster or slower will determin how much it will reproduce and fall off the edges.

##### 4 - Heredity
This is really important too, individuals need to pass their identity to their offspring.
They'll walk around randomly until they meet another unit, then they will breed and create a child with the identity based on the parent values plus a mutation rate we set.
```cs
//Calculating child identity
float childVal = (identity + mateIndividual.identity)/2.0f;
childVal += EvolutionManager.Instance.mutationRate * Random.Range(-1.0f,1.0f);
childVal = Mathf.Clamp(childVal, 0.1f, 1000.0f);
```
To make this model a bit closer to that species concept we talked about, we make sure distant identity values won't be able to breed and produce offspring.
```cs
// Too Different populations cant breed
if (Math.Abs(identity - mateIndividual.identity) >= 75.0f) return;
```
## Simulating

### Setup

We will start out by setting up our global evolution variables, these were found by testing and seeing what looks good and what doesn't.
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/fe9zmw7fom0ve3b0mnze.png)<figcaption>We'll use these values throughout the whole simulation</figcaption>

We also need to define our initial population, in our case, will be 4 units with identity = 800.


![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/67b1ohufcpzr2rmfo96q.png)<figcaption>The initial population, black because their color values were not set yet</figcaption>
 
Let's finally simulate it and see what it will happen!
### Simulation
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/54m578a0a9yevt67n2de.gif)
 
We see that it quickly spreads to all the environment, reaching the 100 max population and keeping it's number around that. 
By letting the simulation run for a bit, we can see a "tendency" in the individual values.
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/w2ok0veufhgauczp7s67.png)
The average values for each environment are not collected but we can clearly see that the units got more red, that means their identity got really close to 1000, so they are faster than when they started out. This is a really good indicator that our model is evolving, because it clearly showed a "tendency" to change given its environment.
What it looks like, is that the faster the units are, they will find more parters and they will breed more, so the faster ones are selected in this environment over the slower ones.

### Isolation

Now, lets get some of these units and separate them into another environment
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/b68uuc57g8u33e7oumt7.gif)
 
And now we isolate them from eachother
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/8hg3qm9khdy656g2htua.gif)
With that, we stop the "_genetic flow_" between the two populations, it means that new traits developed through anagenesis in a population won't be shared between them, if they accumulate enough anagenesis while being isolated we give birth to a new species.

### Cladogenesis

To promote the anagenesis in one of the groups, lets change its environment by removing two of the walls.
_Two walls were removed because removing four of them usually lead to extinction. As we see in nature, if an environment changes too fast, a lot of species go extinct._
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/od5fw818quu8ebld6ymc.gif)

By doing that, being fast is not that good anymore, because being faster means you will fall off more. Let's see how our new group adapts to that.

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/mqlgpd0zvj3xenv8issf.png)
After a few minutes of simulation, we start to notice the new group getting slower, as we expected.
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/ciulx9qpxv9yhojb9c17.png)
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1u6qa8n4q9e2iye2hdvo.png)<figcaption>Removed the last two walls </figcaption>
![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/ux4fs1vr7ztymvktkrkf.gif)
We can now see that the new environment selected slower and slower speeds, and accumulated so much anagenesis that the new group is now totally unrecognizable from its ancestors (or its brothers, since they had no need to change given the environment was the same). The new population is not even able to breed with its brother group anymore. This process gave rise to a brand-new **_species_**, demonstrating, in practice, the concept of Cladogenesis and speciation.
