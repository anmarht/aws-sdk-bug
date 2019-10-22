# Performance Testing to show DynamoDB/Core performance bug

This Repository consists of 2 C# solutions, they execute a multithreaded DynamoDB updates and calculate total time in order to show the performance bug in sdk/src/Core/Amazon.Runtime/Internal/_async/AsyncRunner.cs.

Bugfix here https://github.com/anmarht/aws-sdk-net/commit/61d002a1c36f7dfedbb6b26af54e10626f7a3d1c

Issue on aws-sdk-net https://github.com/aws/aws-sdk-net/issues/766