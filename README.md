strans
======

[![Build Status](https://travis-ci.com/Inventitech/strans.svg?token=1pPnTvKwseJq7cTLeeFE&branch=master)](https://travis-ci.com/Inventitech/strans)

`strans` (string transform) is an intuitive string manipulation
utility for the shell, (primarily Unix, but should be
cross-platform(tm)). The user does not need to know any
programming. All she needs to do is provide strans with a set of
examples. strans will learn transformation rules from these examples
and apply them to the input given on STDIN.

How to Run
==========
You need dotnet to run `strans`.

Examples
========
Convert a list of full names to their initials.

```
printf "Moritz Beller\nGeorgios Gousios" |
dotnet strans.dll -b "First Last" -a "FL"
```

neatly outputs

```
MB
GG
```

However, when we add a third name with a middle name, Andy Emil
Zaidman, things start to break, as this does not appear in the
initials:

```
MB
GG
AZ
```

We can fix this by providing `strans` with another example. We create
a file called example-transformations

```
First Last => FL
Firstname Middlename Lastname => FML
```

and call

```
printf "Moritz Beller\nGeorgios Gousios\nAndy Emil Zaidman" |
dotnet strans.dll --example-file example-transformations
```

And, voila, the output is

```
MB
GG
AEZ
```

Note how `strans` adds the second example and generates a global transformation rule that satisfies all examples given to it. Simply having the last FML example would not be enough, because it would miss the case where only two names are available.


Related Work
============

`strans` uses program-by-example techniques from
[https://microsoft.github.io/prose/](Microsoft PROSE) to come up with
the rules to do this string manipulation. It allows the creation of
extremely complex string transformations within a matter of a few
seconds by just giving easy-to-write examples. In its essence,
`strans` is only a light-weight wrapper around and direct application
of Microsoft's PROSE framework. `strans` provides the goodness of the
now-removed PowerShell (!) command
[https://docs.microsoft.com/en-us/powershell/module/Microsoft.PowerShell.Utility/Convert-String?view=powershell-5.1](Convert-String).
