# Performance Testing to show DynamoDB/Core performance bug

This Repository consists of 2 C# solutions, they execute a multithreaded DynamoDB updates and calculate total time in order to show the performance bug in sdk/src/Core/Amazon.Runtime/Internal/_async/AsyncRunner.cs.

Bugfix here https://github.com/anmarht/aws-sdk-net/commit/61d002a1c36f7dfedbb6b26af54e10626f7a3d1c

Issue on aws-sdk-net https://github.com/aws/aws-sdk-net/issues/766

I don't know why Amazon marked it as a question not a performance bug. The difference in performance is big, and the other reason is causing unnecessary high CPU load. 

Please feel free to use my code as you like, any AWS code is governed by their own license.
