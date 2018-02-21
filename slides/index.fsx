(**
- title : F# Type Provider Demo
- description : A brief primer on F# and a few demos on F# type providers.
- them : night
- transition : default

***

## F# Type Provider Demo

<br />
<br />
Jason Down <br />
Igloo Software <br />
<br />
Feb-21-2018

***

### Agenda

<br />

* Brief F# Primer
    * What is F#
    * Syntax overview
    * Types (relative to demo)
    * Pattern matching basics
    * Demo F# Interactive
* Type Providers
    * What are they?
    * How do they work?
    * Demo SqlProgrammability type provider (via fsx)
* Main Demo w/ Walkthrough
    * Small, but complete solution
        * F# DAL library (w/ friendly C# layer)
        * C# Model library
        * C# Winforms application

***

### F# Primer

##### What is F#?

* .Net Language
    * Access to .Net libraries
    * Interop with other .Net languages
* Strongly typed (inference is stronger than C#)
* Multi-paradigm
    * Functional (first)
    * Imperative
    * Object-Oriented
* Pragmatic
* Inspired many C# features (generics, lambdas, linq, pattern matching)

***

### F# Syntax

##### let Binding

<div class="fragment">
<ul>
<li>F# values are immutable by default (values are bound).
</ul>
*)
let num = 42
num = 1337 // this is a comparison
let numCompare = num = 1337 // i.e. this
(*** include-value : num ***)
(*** include-value : numCompare ***)
(**
</div>

<div class="fragment">
<ul>
<li><i>mutable</i> keyword allows variables.
</ul>
*)
let mutable mutNum = 42
mutNum <- 1337
(*** include-value: mutNum ***)
(**
</div>

---

### F# Syntax

##### Let Binding Con't

<div class="fragment">
<ul>
<li>Types are inferred (still compile-time safe).
</ul>
*)
let myInt = 2
let myFloat = 2.0
(*** include-value : myInt ***)
(*** include-value : myFloat ***)
(**
</div>

---

### F# Syntax

##### Functions

<div class="fragment">
<ul>
<li>Also declared with let bindings.
<li>Uses type inference.
</ul>
*)
let square x = x * x
square 5
(*** include-value : square 5 ***)
(**
</div>

---

### F# Syntax

##### Functions Con't

<div class="fragment">
<ul>
<li>Type annotations for readability (or when compiler isn't sure).
</ul>
*)
let squareFloat (x : float) = x * x
let squareFloat2 (x : float) : float = x * x

squareFloat 4. // or square 4.0
(*** include-value : squareFloat 4. ***)
(**
</div>

<div class="fragment">
<ul>
<li>Can't pass int to float without a cast.
</ul>
*)
squareFloat 4 // compile error
squareFloat (float 4)
(*** include-value : squareFloat 4 ***)
(*** include-value : squareFloat (float 4) ***)
(**
</div>

---

### F# Functions

##### Functions Con't

<div class="fragment">
<ul>
<li>Curried by the compiler.
</ul>
*)
let add x y = x + y
add 5 3
(*** include-value : add 5 3 ***)
(**
</div>

<div class="fragment">
<ul>
<li>Currying allows partial function application.
</ul>
*)
let add10 = add 10
add10 5
(*** include-value : add10 5 ***)
(**
</div>

---

### F# Syntax

##### Pipe Forward Operator <strong>|></strong>

<div class="fragment">
<ul>
<li>Previous evaluation result passed as last argument of next function.
</ul>
<br />
<br />
</div>

<div class="fragment">
<ul>
<li>Without pipe (read from inside out):
</ul>
*)
let myComplicatedFunc x y = 
    (add10 (add x (square y)))

myComplicatedFunc 2 3
(*** include-value : myComplicatedFunc 2 3 ***)
(**
</div>

---

### F# Syntax

##### Pipe Forward Operator Con't

<ul>
<li>With pipe (read left to right, chained transformation):
</ul>
*)
let myComplicatedFunc2 x y =
    y |> square |> add x |> add10
(*** include-value : myComplicatedFunc2 2 3 ***)
(**

---

### F# Syntax

##### Pipe Forward Operator Con't

<div class="fragment">
<ul>
<li>Works well with collection transformation pipelines.
</ul>
*)
let squareEvens =
    [1;2;3;4;5]                     // list comprehension
    |> List.filter (fun x -> x % 2 = 0) // evens
    |> List.map (fun x -> x * x)        // square
(*** include-value : squareEvens ***)
(**
</div>

<div class="fragment">
<br />
<br />
<ul>
<li>Lambda syntax
<li>List comprehensions (also works for seq (IEnumerable) and arrays).
<li>There are other pipe operators (||> <| <||).
</div>

---

### F# Syntax Composition Operator <strong>>></strong>

<div class="fragment">
<ul>
<li>Output of one function is input to next function (lego).
<li>Useful to prevent multiple iterations.
</ul>
*)
let dbl x = x + x
let sq x = x * x

[1;2;3;4;5]
|> List.map (sq >> dbl >> string) // performs all three
(*** include-value : [1;2;3;4;5] |> List.map (sq >> dbl >> string) ***)
(**
</div>

<div class="fragment">
<ul>
<li>Can also create a composed function.
</ul>
*)
let sqDblStr = sq >> dbl >> string

[1;2;3;4;5]
|> List.map sqDblStr
(*** include-value : [1;2;3;4;5] |> List.map sqDblStr ***)
(**
</div>

***

### F# Types

##### Records

<div class="fragment">
<ul>
<li>Immutable classes (all properties getters only).
<li>Take all properties in constructor.
*)
type Person = {
    Age : int
    Name : string
    Skillz : string seq
}

let p1 = { Person.Age = 39
           Person.Name = "Jason"
           Person.Skillz = ["f#"; "c#"]}
// more succinct (No need for Person.Field)
let p2 = { Age = 39; Name = "Jason"; Skillz = ["f#"; "c#"] }

p1.Name
(*** include-value : p1.Name ***)
(**
</div>

---

### F# Types

##### Records Con't

<div class="fragment">
<ul>
<li>Missing a field will not compile.
</ul>
*)
let yoda = { Person.Age = 900; Skillz = ["lightsaber"; "the force"]}
// Compiler Error: No assignment given for field 'Name' of type 'Person'
(*** include-value : yoda ***)
(**
</div>

---

### F# Types

##### Records Con't

<div class="fragment">
<ul>
<li>Copy and update constructor.
</ul>
*)
let p3 = {p1 with Name = "Jason Clone"}
(**
</div>

<div class="fragment">
<ul>
<li>Structural equality (many interfaces implemented for you).
</ul>
*)
let p1Equalsp2 = p1 = p2
let p1Equalsp3 = p1 = p3
(*** include-value : p1Equalsp2 ***)
(*** include-value : p1Equalsp3 ***)
(**
</div>

---

### F# Types

##### Records Con't

<div class="fragment">
<ul>
<li>Equivalent C# class (definitions only; override code elided).
</ul>

    [lang=cs]
    public sealed class Person : IEquatable<Person>, IStructuralEquatable, 
        IComparable<Person>, IComparable, IStructuralComparable
    {
        public Person(int id, string name, IEnumerable<string> friends);
        public int Age { get; }
        public string Name { get; }
        public IEnumerable<string> Skillz { get; }
        public sealed override int CompareTo(Person obj);
        public sealed override int CompareTo(object obj);
        public sealed override int CompareTo(object obj, IComparer comp);
        public sealed override bool Equals(object obj, IEqualityComparer comp);
        public sealed override bool Equals(Person obj);
        public sealed override bool Equals(object obj);
        public sealed override int GetHashCode(IEqualityComparer comp);
        public sealed override int GetHashCode();
        public override string ToString();
    }

 </div>

---

### F# Types

##### Options

<div class="fragment"
<ul>
<li>Specialized discriminated Union (not covering due to time).
<li>Option has some value or no value.
*)
// Commented out or it will affect script later on
//
//  type Option<'a> = 
//      | Some of 'a
//      | None

// Many ways to create 
let myValidInt = Some 1
let myValidString = "I am a value" |> Some
let noValue = None
(**
</div>

---

### F# Types

##### Options Con't

<div class="fragment">
<ul>
<li>Multiple ways to declare in your types.
</ul>
*)
type SearchResult = {
    SearchTerm : string
    Results : string list option    // built-in postfix keyword
    Links : List<Option<string>>    // C# Generics style
}
(**
</div>

---

### F# Types

##### Options Con't

<div class="fragment">
<ul>
<li>Example usage:
</ul>
*)
let find3 = [1..5] |> List.tryFind (fun x -> x = 3) // Some 3
let find7 = [1..5] |> List.tryFind (fun x -> x = 7) // None
(*** include-value : find3 ***)
(*** include-value : find7 ***)
(**
</div>

***

### F# Pattern Matching (Basics only)

<div class="fragment">
<ul>
<li>Matches the shape of data (evaluates first).
<li> Ensures all match conditions are covered.
</ul>
*)
let myIntValue1 = 5
let myIntValue2 = 10
// silly example, use if/then/else
let uselessMatchButPossible =
    match myIntValue1 > myIntValue2 with
    | true -> "5 is bigger than 10"
    | false -> "5 is not bigger than 10"
(*** include-value : uselessMatchButPossible ***)
(**
</div>

---

### F# Pattern Matching Con't

<div class="fragment">
<ul>
<li>Use with deconstruction to be more useful.
<li>Deconstruction works great to extract option types.
</ul>
*)
let myOptionVal = 
    [1;2;3;4]
    |> List.tryFind (fun x -> x = 3)

// Can also add when clauses

let betterExample = 
    match myOptionVal with
    | Some n when n > (System.Int32.MaxValue / 2)
        -> sprintf "%i can't be doubled." n
    | Some n -> sprintf "%i doubled is %i" n (n*2)
    | None -> sprintf "Can't do it!"
(*** include-value : myOptionVal ***)
(*** include-value : betterExample ***)
(**
</div>

---

### F# Pattern Matching

##### Shameless plug:
For more detailed overview of Pattern Matching, check out my blog: <br /><br />
[F# Pattern Matching Part 1 - Introduction](http://www.jason-down.com/2017/01/10/f-pattern-matching-part-1/) <br />
[F# Pattern matching Part 2 - Active Patterns](http://www.jason-down.com/2017/01/24/f-pattern-matching-part-2-active-patterns/)

*** 

### F# Interative (FSI)

<div class="fragment">
<ul>
<li>Interactive terminal.
<li>Let's you test code as you write it.
<li>Eliminates <em>some</em> unit testing.
<li>Promotes writing smaller composable functions.
</ul>
<br />
<br />
Workflow: Scripts (fsx) -> Test out functions -> Convert to library (fs).
</div>

---

### F# Interative (FSI) Con't

<br />
<br />

## DEMO

*)
