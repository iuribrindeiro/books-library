This project was written using some principles of OO and FP.

1. It makes heavy use of the [ErrorOr](https://github.com/amantinband/error-or) package to deal with 
basic domain failures
2. The only classes that hold state are Repositories and the UnitOfWork. 
Most classes are static and are made of pure functions. This means that most methods/functions 
are writen in a way that it will return the same value if given the same input.
This makes the function easy to test.
3. I just made use of Controllers to demonstrate how it will require more code and will provide 0 advantages in practice, 
since minimal APIs will do the same work with less code.
4. The way it implements "Domain Events" is that every domain command 
will return a result with also a list of events that happened inside that action.
(I called it commands, but it is not exactly a CQRS approach, just named it to make sure it follows a pattern)
5. The UnitOfWork SaveChangesAsync method is responsible for persisting the changes after any domain logic is executed.
This method is not only about DB changes, but also notifiying all domain events previously created. 
It doesn't have to notify anything directly, it could just call a external function.
