# Update MSRP 

	Document how you would go about changing the Base MSRP for all Tesla Model Y vehicles and the assumptions made.

```
UPDATE evs.models
SET base_msrp = <Pick your value>
FROM evs.makes
WHERE evs.makes.manufacturer = 'TESLA'
  AND evs.models.model_name = 'MODEL Y'
  AND evs.models.make_id = evs.makes.id;
```

I'll note that since these models are distinct per year, this will probably not be what you want, the actual values will probably be more complex, and you may want to issue multiple updates for each year/model.
