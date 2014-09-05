// conditional logic works as follows:

// given an object like the Transaction class
// one could filter out unwanted instances by encapsulating a list of conditions which the set must satisfy

filter = [{
    beginDate: [{
        between: ["2012-01-01", "2014-12-31"]
    }]
}];

// beginDate between 2012-01-01 and 2014-12-31


// filter is an array of conditions each element in the array represents a logical OR condition
// if an element in the array is an array its elements represent a list of logical OR conditions:

filter = [{
    beginDate: [{
        between: ["2012-01-01", "2014-12-31"]
    }, {
        equals: ["2014-06-01"]
    }]
}]

// beginDate between 2012-01-01 AND 2014-12-31 OR beginDate equals 2014-06-01

filter = [{
    beginDate: [{
        between: ["2012-01-01", "2014-12-31"],
        context: {
            DayOfWeek: {
                equals: "Monday"
            }
        }
    }]
}];

// beginDate between 2012-01-01 AND 2014-12-31 AND beginDate.DayOfWeek == Monday
