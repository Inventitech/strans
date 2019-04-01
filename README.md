strans
======

[![Build Status](https://travis-ci.com/Inventitech/strans.svg?token=1pPnTvKwseJq7cTLeeFE&branch=master)](https://travis-ci.com/Inventitech/strans)
![LOC](https://tokei.rs/b1/github/inventitech/strans)
[![Github Releases](https://img.shields.io/github/downloads/inventitech/strans/total.svg)](https://github.com/Inventitech/strans/releases)
[![Join the chat at https://gitter.im/inventitech/strans](https://badges.gitter.im/inventitech/strans.svg)](https://gitter.im/inventitech/strans?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


`strans` (string transform) is an intuitive string manipulation
utility for the shell (primarily Unix, but should work™
cross-platform). Users do not need to know anything about
programming. All they need to do is provide `strans` with a set of
examples. `strans` will automagically learn transformation rules from
these examples and apply them to the input given on STDIN.

How to install
==============

`strans` is distributed platform indepedently as a NuGet package, or
(if you do not want to install dotnet) standalone as an AppImage for
Linux and as a DMG for MacOs.  Download the latest `strans` from
[releases](https://github.com/Inventitech/strans/releases).

## Platform-independent global tool (requires `dotnet`)

`strans` is available as a 
[global tool on NuGet](https://www.nuget.org/packages/strans). If you 
have `dotnet` installed, you can simply `dotnet tool install -g strans` 
to install it (or use `dotnet tool update`, `uninstall` etc.).

## Linux

Our AppImage should run on practically any recent linux Desktop
distribution.

After downloading, simply do

`
chmod +x strans-linux.AppImage
./strans-linux.AppImage
`

To install it system-wide as `strans`, just

`sudo cp strans-linux.AppImage /usr/bin/strans`

## MacOs
After mounting the `dmg` and copying `strans.app` into `/Applications`, it might be necessary to

```
chmod +x /Applications/strans.app/Contents/MacOS/strans
/Applications/strans.app/Contents/MacOS/strans
```
to run `strans`.
 

How to use
==========
```
# With before and after example
strans -b pattern-to-match -a desired-transformation

# With file that contains examples
strans -f file-with-examples

# Help page
strans --help
```

Examples
========
![Strans in action](https://github.com/Inventitech/strans/raw/master/strans.gif)

## Example 1: Extract ending of files

Assume that

```
ls
Document.pdf  Document2.pdf Document.txt  Document.png
```

Now we want to get a unique list of all file endings present in the directory:

```
ls | strans -b Document.pdf -a pdf | sort -u
```

Note how nicely strans (here defined as an alias) integrates with other tools.

Of course, as
[StackOverflow](https://stackoverflow.com/questions/1842254/how-can-i-find-all-of-the-distinct-file-extensions-in-a-folder-hierarchy)
will tell you, we could obtain the same result with

```
ls | perl -ne 'print $1 if m/\.([^.\/]+)$/' | sort -u 
```

But with `strans` we accomplished the same with much less brain work,
without StackOverflow and Perl, but instead with pure joy!

## Example 2: Convert full names to their initials

```
printf "Moritz Beller\nGeorgios Gousios" |
strans -b "First Last" -a "FL"
```

neatly outputs

```
MB
GG
```

However, when we add a third entry with a middle name, Andy Emil
Zaidman, things start to break, as this does not appear in the
initials:

```
MB
GG
AZ
```

We can fix this by providing `strans` with another example. We create
a file called `example-transformations`

```
First Last => FL
Firstname Middlename Lastname => FML
```

and call

```
printf "Moritz Beller\nGeorgios Gousios\nAndy Emil Zaidman" |
strans --example-file example-transformations
```

And, voila, the output is

```
MB
GG
AEZ
```

Note how `strans` adds the second example and generates a global
transformation rule that satisfies all examples given to it. Simply
having the last FML example would not be enough because it would miss
the case where only two names are available.

How to develop
==============

You need [dotnet](https://dotnet.microsoft.com/) to run `strans`.

```
git clone https://github.com/Inventitech/strans.git
cd strans
dotnet restore
dotnet publish -c Release
```

An alias (in your bashrc, ...) can make `strans` integrate seamlessly in
a Unix environment:

```
alias strans="dotnet path/to/strans.dll"
```


Background
==========

`strans` uses program-by-example techniques from [Microsoft
PROSE](https://microsoft.github.io/prose/) to come up with the rules
behind this string manipulation. PROSE allows the creation of
extremely complex string transformations within a matter of a few
seconds by just giving easy-to-write examples. In its essence,
`strans` is only a light-weight wrapper around and direct application
of Microsoft's PROSE framework. `strans` provides the goodness of the
now-removed PowerShell (!) command
[Convert-String](https://docs.microsoft.com/en-us/powershell/module/Microsoft.PowerShell.Utility/Convert-String?view=powershell-5.1).
