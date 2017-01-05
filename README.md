# Introduction

Ifp.Validation is a library for validating an object against a set of 
rules and encapsulating the validation result in an easily presentable form:

![ValidationSummaryPresenterExample](Documentation/Media/ValidationSummaryPresenterExample.png)

## Design goals

The library was created with the following design goals:
<dl>
    <dt>Clear focus</dt>
    <dd>
        Provide a library that can validate an object and be able to present the result of several validation rules at once
        by also providing a notion of validation rule violation severity.
    </dd>
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
        This leads to great flexibility and a good testability of the validation rules.
    </dd>
    <dt><a href="https://de.wikipedia.org/wiki/Dependency_Injection">Dependency injection</a></dt>
    <dd>
        The library works best in combination with a <em>dependency injection framework</em>.
        The dependencies are expressed by constructor parameters (see the examples below).
    </dd>
    <dt>Reuse and combinability of validation rules</dt>
    <dd>
        Validation rules can easily be combined and reused even if the type of the object to validate
        is not of the type that the validation rule demands (see below for examples).
    </dd>
    <dt>Extensibility</dt>
    <dd>
        All validation steps can be used as entry points for own implementations and extensions.        
    </dd>
</dl>

The following parts are not included in the library:

* No predefined validation rules, like *mandatory field*, *email address*, *Max length* and alike.
* No presentation logic. A WPF library with an `IValidationSummaryPresentationService` can
  be found at [github.com/ifpanalytics/Ifp.Validation.WPF](https://github.com/ifpanalytics/Ifp.Validation.WPF)

## How to use

The first step in the use of the library is to have an  *object to validate*:

```CS
public class RegisterNewUserModel
{
    public string EMail { get; set; }
    public string GivenName { get; set; }
    public string SurName { get; set; }
    public DateTime? BithDate { get; set; }
}
```     

Then one ore more validation rules are defined:

```CS
public class BirthdateValidationRule : ValidationRule<RegisterNewUserModel>
{
    public override ValidationOutcome ValidateObject(RegisterNewUserModel objectToValidate)
    {
        // Use the ToFailure extension method for strings to create a ValidationOutcome.
        if (objectToValidate.BithDate == null)
            return "You did not enter a birth date. You will not be able to use some of our services.You can add this information later.".ToFailure(FailureSeverity.Information);
        // Return ValidationOutcome.Success to indicate success.
        return ValidationOutcome.Success;
    }
}

public class PasswordValidationRule : ValidationRule<RegisterNewUserModel>
{
    // Import other services per constructor injection if needed
    public PasswordValidationRule(IPasswordPolicyVerifier passwordPolicyVerifier)
    {
        PasswordPolicyVerifier=passwordPolicyVerifier;
    }

    protected IPasswordPolicyVerifier PasswordPolicyVerifier { get; }

    public override ValidationOutcome ValidateObject(RegisterNewUserModel objectToValidate)
    {
        if (objectToValidate.Password != objectToValidate.PasswordRepeated)
            return "The two passwords you entered are not the same.".ToFailure(FailureSeverity.Error);
        if (!PasswordPolicyVerifier.ConformsToPolicy(objectToValidate.Password))
            return "The password you entered does not conform to the password policy.".ToFailure(FailureSeverity.Error);
        if (PasswordPolicyVerifier.IsWeakPassword(objectToValidate.Password))
            return "The password you entered is valid but weak. Do you want to use it anyway?".ToFailure(FailureSeverity.Warning);
        return ValidationOutcome.Success;
    }
}
```

The rules can be combined to a set of validations:

```CS
public class RegisterNewUserValidator : RuleBasedValidator<RegisterNewUserModel>
{
    public RegisterNewUserValidator(PasswordValidationRule passwordValidationRule, BirthdateValidationRule birthdateValidationRule)
        : base(passwordValidationRule, birthdateValidationRule)
    {

    }
}
```

This validator can be used to produce a `ValidationSummary` and this summary can be presented to the user.

```CS
public class RegisterNewUserService: IRegisterNewUserService
{
    public RegisterNewUserService(RegisterNewUserValidator validator, IValidationSummaryPresentationService validationSummaryPresentationService)
    {
        Validator = validator;
        ValidationSummaryPresentationService = validationSummaryPresentationService;
    }

    protected IValidationSummaryPresentationService ValidationSummaryPresentationService { get; }
    protected RegisterNewUserValidator Validator { get; }

    public bool ValidateAndStoreNewUser(RegisterNewUserModel model)
    {
        var summary = Validator.Validate(model);
        if (ValidationSummaryPresentationService.ShowValidationSummary(summary))
            // There was no error or the user pressed 'OK'.
            return false;
        // Logic to store the model to the database.
        return true;
    }
}
```

To construct a new `RegisterNewUserService` all the services need to be resolved:

```CS
new RegisterNewUserService(new RegisterNewUserValidator(new PasswordValidationRule(new PasswordPolicyVerifier()), new BirthdateValidationRule()), new ValidationSummaryPresentationService()); 
```

This cumbersome work is best delegated to a dependency injection framework like [Ninject](http://www.ninject.org/) or [Unity](https://github.com/unitycontainer/unity).

## Understanding `ValidationOutcome`

## Reusing `ValidationRule`

## How to get
