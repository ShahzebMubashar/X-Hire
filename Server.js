const express = require('express'),
    dbOperations=require('./dFiles/dbOperations'),
    cors = require('cors');

dbOperations.getEmployees().then(res => {
    console.log(res);
})