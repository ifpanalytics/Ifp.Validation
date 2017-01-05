# Introduction

Ifp.Validation is a library for validating an object against a set of 
rules and encapsulating the validation result in an easily presentable form:

![ValidationSummaryPresenterExample](Documentation/Media/ValidationSummaryPresenterExample.png)

## Design goals

The library was created with the following design goals:
<dl>
    <dt><a href="https://en.wikipedia.org/wiki/Separation_of_concerns">Separation of concerns</a></dt>
    <dd>
        The libraray separates the <em>object to validate</em> from the validation logic. 
        There is no need for the <em>object to validate</em> to implement an interface or 
        to be annotated with attributes.
    </dd>
    <dt><a href="https://en.wikipedia.org/wiki/Single_responsibility_principle">Single responsibility principle</a></dt>
    <dd>
        The library distinguishes between the different validation steps:
        <dl>
            <dt>ValidationRule</dt>
            <dd>
                A small unit with a definitive purpose returning a <em>validation outcome</em> 
                that describes the result of the validation.
            </dd>
            <dt>ValidationOutcome</dt>
            <dd>
                An object providing a text description of the violation of a rule and a 
                <em>severity</em> of the violation. The severity can be <em>information</em>, 
                <em>warning</em> or <em>error</em> (see later for details). 
            </dd>
            <dt>Validator</dt>
            <dd>
                A class that takes one or more <em>ValidationRules</em> and combines them
                to produce a <em>ValidationSummary</em>.
            </dd>
            <dt>ValidationSummaryPresenter</dt>
            <dd>
                A service that takes a <em>ValidationSummary</em> and presents it to the user (see screenshot above).
            </dd>
        </dl>
    </dd>
    <dt><a href="https://de.wikipedia.org/wiki/Dependency_Injection">Dependency injection</a></dt>
    <dd>
        The library works best in combination with a <em>dependency injection framework</em>.
        The dependencies are expressed by constructor parameters (see the examples below).
    </dd>
    <dt>Reuse and combinability of validation rules</dt>
    <dd>fff</dd>
    <dt>Extensibility</dt>
    <dd>fff</dd>

</dl>

## How to use

## How to get
dd