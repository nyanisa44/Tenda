/**
 *  @description        Represents a model that's to be used in the current scope.
 *	@param fields	    {Array} => [,'string']
 *                      The fields/properties to be attached to the current model.
 *	@param validation 	{Function} => {bool}
 *                      The methods that checks the validity of the current model.
 */
function Model(fields, validation)
{
    for (var key in fields)
        Object.defineProperty(this, fields[key], { value: undefined, writable: true, enumerable: true });

	Object.defineProperty(this, "IsValid", { get: validation });
}

